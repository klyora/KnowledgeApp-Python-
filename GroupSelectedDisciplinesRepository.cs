using KnowledgeApp.DataAccess.Context;
using KnowledgeApp.DataAccess.Entities;
using KnowledgeApp.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeApp.DataAccess.Repositories
{
    public class GroupSelectedDisciplinesRepository
    {
        private readonly KnowledgeTestDbContext _context;

        public GroupSelectedDisciplinesRepository(KnowledgeTestDbContext context)
        {
            _context = context;
        }

        public async Task<List<GroupSelectedDisciplinesModel>> GetAll()
        {
            var entities = await _context.GroupSelectedDisciplines
                .AsNoTracking()
                .ToListAsync();

            return entities.Select(e => ToModel(e)).ToList();
        }

        public async Task<GroupSelectedDisciplinesModel> GetById(int id)
        {
            var entity = await _context.GroupSelectedDisciplines.FindAsync(id);
            if (entity == null) throw new Exception($"Запись выбора дисциплин с ID {id} не найдена");
            return ToModel(entity);
        }

        // Загрузка сохранённого выбора при повторном входе на страницу.
        // Возвращает null если выбор ещё не был сделан.
        public async Task<GroupSelectedDisciplinesModel?> GetByGroupAndSemester(int groupId, int semesterId)
        {
            var entity = await _context.GroupSelectedDisciplines
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.GroupId == groupId && x.SemesterId == semesterId);

            return entity == null ? null : ToModel(entity);
        }

        public async Task<List<GroupSelectedDisciplinesModel>> GetByGroupId(int groupId)
        {
            var entities = await _context.GroupSelectedDisciplines
                .Where(x => x.GroupId == groupId)
                .AsNoTracking()
                .ToListAsync();

            return entities.Select(e => ToModel(e)).ToList();
        }

        // Подгрузка актуальных дисциплин из учебной программы группы.
        // Цепочка: study_groups → study_programs → departments → disciplines
        public async Task<List<DisciplineForGroupModel>> GetAvailableDisciplinesForGroup(int groupId)
        {
            if (!_context.StudyGroups.Any(g => g.Id == groupId))
                throw new Exception($"Учебная группа с ID {groupId} не существует");

            var result = await (
                from sg  in _context.StudyGroups   where sg.Id == groupId
                join sp  in _context.StudyPrograms on sg.StudyProgramId equals sp.Id
                join dep in _context.Departments   on sp.DepartmentId   equals dep.Id
                join dis in _context.Disciplines   on dep.Id            equals dis.DepartmentId
                select new DisciplineForGroupModel
                {
                    DisciplineId   = dis.Id,
                    DisciplineName = dis.Name,
                    DepartmentId   = dep.Id,
                    DepartmentName = dep.Name
                }
            ).AsNoTracking().ToListAsync();

            return result;
        }

        public async Task<GroupSelectedDisciplinesModel> Create(GroupSelectedDisciplinesModel model, int managerUserId)
        {
            ValidateModel(model);

            var (dept1Id, isOwn1, dept2Id, isOwn2) = await ResolveOwnership(model, managerUserId);

            var entity = new GroupSelectedDisciplines
            {
                GroupId                      = model.GroupId,
                SemesterId                   = model.SemesterId,
                Discipline1Id                = model.Discipline1Id,
                Discipline1OwnerDepartmentId = dept1Id,
                Discipline1IsOwn             = isOwn1,
                Discipline2Id                = model.Discipline2Id,
                Discipline2OwnerDepartmentId = dept2Id,
                Discipline2IsOwn             = isOwn2,
                SelectedBy                   = model.SelectedBy,
                UpdatedAt                    = DateTime.UtcNow
            };

            await _context.GroupSelectedDisciplines.AddAsync(entity);
            await _context.SaveChangesAsync();

            return ToModel(entity);
        }

        public async Task<GroupSelectedDisciplinesModel> Update(GroupSelectedDisciplinesModel model, int managerUserId)
        {
            var entity = await _context.GroupSelectedDisciplines.FindAsync(model.Id);
            if (entity == null) throw new Exception($"Запись выбора дисциплин с ID {model.Id} не найдена");

            ValidateModel(model);

            var (dept1Id, isOwn1, dept2Id, isOwn2) = await ResolveOwnership(model, managerUserId);

            entity.Discipline1Id                = model.Discipline1Id;
            entity.Discipline1OwnerDepartmentId = dept1Id;
            entity.Discipline1IsOwn             = isOwn1;
            entity.Discipline2Id                = model.Discipline2Id;
            entity.Discipline2OwnerDepartmentId = dept2Id;
            entity.Discipline2IsOwn             = isOwn2;
            entity.SelectedBy                   = model.SelectedBy;
            entity.UpdatedAt                    = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ToModel(entity);
        }

        // Upsert: создать, если нет — обновить, если есть.
        public async Task<GroupSelectedDisciplinesModel> Upsert(GroupSelectedDisciplinesModel model, int managerUserId)
        {
            ValidateModel(model);

            var (dept1Id, isOwn1, dept2Id, isOwn2) = await ResolveOwnership(model, managerUserId);

            var entity = await _context.GroupSelectedDisciplines
                .FirstOrDefaultAsync(x => x.GroupId == model.GroupId && x.SemesterId == model.SemesterId);

            if (entity == null)
            {
                entity = new GroupSelectedDisciplines { GroupId = model.GroupId, SemesterId = model.SemesterId };
                await _context.GroupSelectedDisciplines.AddAsync(entity);
            }

            entity.Discipline1Id                = model.Discipline1Id;
            entity.Discipline1OwnerDepartmentId = dept1Id;
            entity.Discipline1IsOwn             = isOwn1;
            entity.Discipline2Id                = model.Discipline2Id;
            entity.Discipline2OwnerDepartmentId = dept2Id;
            entity.Discipline2IsOwn             = isOwn2;
            entity.SelectedBy                   = model.SelectedBy;
            entity.UpdatedAt                    = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return ToModel(entity);
        }

        public async Task<bool> Delete(int id)
        {
            var entity = await _context.GroupSelectedDisciplines.FindAsync(id);
            if (entity == null) throw new Exception($"Запись выбора дисциплин с ID {id} не найдена");

            _context.GroupSelectedDisciplines.Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }

        // ----------------------------------------------------------------
        // Логика определения «своей / чужой» кафедры
        //
        //  1. Берём faculty_id руководителя из таблицы users.
        //  2. Берём первую кафедру этого факультета из departments.
        //  3. Для каждой дисциплины смотрим disciplines.department_id.
        //  4. Совпадение dept руководителя = dept дисциплины → is_own = true.
        // ----------------------------------------------------------------
        private async Task<(int? dept1Id, bool? isOwn1, int? dept2Id, bool? isOwn2)>
            ResolveOwnership(GroupSelectedDisciplinesModel model, int managerUserId)
        {
            var manager = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == managerUserId)
                ?? throw new Exception($"Пользователь с ID {managerUserId} не найден");

            var managerDeptId = await _context.Departments
                .AsNoTracking()
                .Where(d => d.FacultyId == manager.FacultyId)
                .Select(d => (int?)d.Id)
                .FirstOrDefaultAsync();

            int? dept1Id = null; bool? isOwn1 = null;
            if (model.Discipline1Id.HasValue)
            {
                dept1Id = await GetDisciplineDeptId(model.Discipline1Id.Value);
                isOwn1  = managerDeptId.HasValue && dept1Id == managerDeptId;
            }

            int? dept2Id = null; bool? isOwn2 = null;
            if (model.Discipline2Id.HasValue)
            {
                dept2Id = await GetDisciplineDeptId(model.Discipline2Id.Value);
                isOwn2  = managerDeptId.HasValue && dept2Id == managerDeptId;
            }

            return (dept1Id, isOwn1, dept2Id, isOwn2);
        }

        private async Task<int?> GetDisciplineDeptId(int disciplineId)
        {
            var disc = await _context.Disciplines
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == disciplineId)
                ?? throw new Exception($"Дисциплина с ID {disciplineId} не найдена");

            return disc.DepartmentId;
        }

        private void ValidateModel(GroupSelectedDisciplinesModel model)
        {
            if (model.GroupId == 0 || model.SemesterId == 0)
                throw new Exception("Необходимо указать GroupId и SemesterId");

            if (!_context.StudyGroups.Any(g => g.Id == model.GroupId))
                throw new Exception($"Учебная группа с ID {model.GroupId} не существует");

            if (!_context.Semesters.Any(s => s.Id == model.SemesterId))
                throw new Exception($"Семестр с ID {model.SemesterId} не существует");

            if (model.Discipline1Id.HasValue && !_context.Disciplines.Any(d => d.Id == model.Discipline1Id))
                throw new Exception($"Дисциплина с ID {model.Discipline1Id} не существует");

            if (model.Discipline2Id.HasValue && !_context.Disciplines.Any(d => d.Id == model.Discipline2Id))
                throw new Exception($"Дисциплина с ID {model.Discipline2Id} не существует");

            if (model.Discipline1Id.HasValue && model.Discipline1Id == model.Discipline2Id)
                throw new Exception("Нельзя выбрать одну и ту же дисциплину дважды");
        }

        private GroupSelectedDisciplinesModel ToModel(GroupSelectedDisciplines entity)
        {
            return new GroupSelectedDisciplinesModel(
                entity.Id,
                entity.GroupId,
                entity.SemesterId,
                entity.Discipline1Id,
                entity.Discipline1OwnerDepartmentId,
                entity.Discipline1IsOwn,
                entity.Discipline2Id,
                entity.Discipline2OwnerDepartmentId,
                entity.Discipline2IsOwn,
                entity.SelectedBy,
                entity.UpdatedAt);
        }
    }

    // DTO для списка доступных дисциплин группы
    public class DisciplineForGroupModel
    {
        public int    DisciplineId   { get; set; }
        public string DisciplineName { get; set; } = string.Empty;
        public int    DepartmentId   { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
    }
}
