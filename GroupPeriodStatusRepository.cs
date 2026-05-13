using KnowledgeApp.DataAccess.Context;
using KnowledgeApp.DataAccess.Entities;
using KnowledgeApp.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeApp.DataAccess.Repositories
{
    public class GroupPeriodStatusRepository
    {
        private readonly KnowledgeTestDbContext _context;

        public GroupPeriodStatusRepository(KnowledgeTestDbContext context)
        {
            _context = context;
        }

        public async Task<List<GroupPeriodStatusModel>> GetAll()
        {
            var entities = await _context.GroupPeriodStatuses
                .AsNoTracking()
                .ToListAsync();

            return entities.Select(e => ToModel(e)).ToList();
        }

        public async Task<GroupPeriodStatusModel> GetById(int id)
        {
            var entity = await _context.GroupPeriodStatuses.FindAsync(id);
            if (entity == null) throw new Exception($"Статус участия с ID {id} не найден");
            return ToModel(entity);
        }

        public async Task<GroupPeriodStatusModel?> GetByGroupAndSemester(int groupId, int semesterId)
        {
            var entity = await _context.GroupPeriodStatuses
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.GroupId == groupId && x.SemesterId == semesterId);

            return entity == null ? null : ToModel(entity);
        }

        public async Task<List<GroupPeriodStatusModel>> GetByGroupId(int groupId)
        {
            var entities = await _context.GroupPeriodStatuses
                .Where(x => x.GroupId == groupId)
                .AsNoTracking()
                .ToListAsync();

            return entities.Select(e => ToModel(e)).ToList();
        }

        public async Task<GroupPeriodStatusModel> Create(GroupPeriodStatusModel model)
        {
            ValidateModel(model);

            var entity = new GroupPeriodStatus
            {
                GroupId    = model.GroupId,
                SemesterId = model.SemesterId,
                Status     = model.Status,
                UpdatedBy  = model.UpdatedBy,
                UpdatedAt  = DateTime.UtcNow
            };

            await _context.GroupPeriodStatuses.AddAsync(entity);
            await _context.SaveChangesAsync();

            return ToModel(entity);
        }

        public async Task<GroupPeriodStatusModel> Update(GroupPeriodStatusModel model)
        {
            var entity = await _context.GroupPeriodStatuses.FindAsync(model.Id);
            if (entity == null) throw new Exception($"Статус участия с ID {model.Id} не найден");

            ValidateModel(model);

            entity.Status    = model.Status;
            entity.UpdatedBy = model.UpdatedBy;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ToModel(entity);
        }

        // Upsert: создать, если нет — обновить, если есть.
        // Используется при установке статуса со страницы.
        public async Task<GroupPeriodStatusModel> Upsert(GroupPeriodStatusModel model)
        {
            ValidateModel(model);

            var entity = await _context.GroupPeriodStatuses
                .FirstOrDefaultAsync(x => x.GroupId == model.GroupId && x.SemesterId == model.SemesterId);

            if (entity == null)
            {
                entity = new GroupPeriodStatus { GroupId = model.GroupId, SemesterId = model.SemesterId };
                await _context.GroupPeriodStatuses.AddAsync(entity);
            }

            entity.Status    = model.Status;
            entity.UpdatedBy = model.UpdatedBy;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return ToModel(entity);
        }

        public async Task<bool> Delete(int id)
        {
            var entity = await _context.GroupPeriodStatuses.FindAsync(id);
            if (entity == null) throw new Exception($"Статус участия с ID {id} не найден");

            _context.GroupPeriodStatuses.Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }

        private void ValidateModel(GroupPeriodStatusModel model)
        {
            if (model.GroupId == 0 || model.SemesterId == 0)
                throw new Exception("Необходимо указать GroupId и SemesterId");

            if (!_context.StudyGroups.Any(g => g.Id == model.GroupId))
                throw new Exception($"Учебная группа с ID {model.GroupId} не существует");

            if (!_context.Semesters.Any(s => s.Id == model.SemesterId))
                throw new Exception($"Семестр с ID {model.SemesterId} не существует");

            if (model.UpdatedBy.HasValue && !_context.Users.Any(u => u.Id == model.UpdatedBy))
                throw new Exception($"Пользователь с ID {model.UpdatedBy} не существует");
        }

        private GroupPeriodStatusModel ToModel(GroupPeriodStatus entity)
        {
            return new GroupPeriodStatusModel(
                entity.Id,
                entity.GroupId,
                entity.SemesterId,
                entity.Status,
                entity.UpdatedBy,
                entity.UpdatedAt);
        }
    }
}
