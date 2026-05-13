using KnowledgeApp.DataAccess.Repositories;
using KnowledgeApp.Core.Models;

namespace KnowledgeApp.Application.Services
{
    public class GroupSelectedDisciplinesService
    {
        private readonly GroupSelectedDisciplinesRepository _groupSelectedDisciplinesRepository;

        public GroupSelectedDisciplinesService(GroupSelectedDisciplinesRepository groupSelectedDisciplinesRepository)
        {
            _groupSelectedDisciplinesRepository = groupSelectedDisciplinesRepository;
        }

        public async Task<List<GroupSelectedDisciplinesModel>> GetAll()
        {
            return await _groupSelectedDisciplinesRepository.GetAll();
        }

        public async Task<GroupSelectedDisciplinesModel> GetById(int id)
        {
            return await _groupSelectedDisciplinesRepository.GetById(id);
        }

        // Загрузка ранее сохранённого выбора при повторном входе на страницу.
        // Возвращает null если выбор ещё не был сделан.
        public async Task<GroupSelectedDisciplinesModel?> GetByGroupAndSemester(int groupId, int semesterId)
        {
            return await _groupSelectedDisciplinesRepository.GetByGroupAndSemester(groupId, semesterId);
        }

        public async Task<List<GroupSelectedDisciplinesModel>> GetByGroupId(int groupId)
        {
            return await _groupSelectedDisciplinesRepository.GetByGroupId(groupId);
        }

        // Получить список дисциплин из учебной программы группы для выпадающих списков.
        public async Task<List<DisciplineForGroupModel>> GetAvailableDisciplinesForGroup(int groupId)
        {
            return await _groupSelectedDisciplinesRepository.GetAvailableDisciplinesForGroup(groupId);
        }

        // Сохранить выбор дисциплин.
        // Бэкенд сам определит кафедру каждой дисциплины и проставит флаги IsOwn.
        public async Task<GroupSelectedDisciplinesModel> Create(GroupSelectedDisciplinesModel model, int managerUserId)
        {
            return await _groupSelectedDisciplinesRepository.Create(model, managerUserId);
        }

        public async Task<GroupSelectedDisciplinesModel> Update(GroupSelectedDisciplinesModel model, int managerUserId)
        {
            return await _groupSelectedDisciplinesRepository.Update(model, managerUserId);
        }

        public async Task<GroupSelectedDisciplinesModel> Upsert(GroupSelectedDisciplinesModel model, int managerUserId)
        {
            return await _groupSelectedDisciplinesRepository.Upsert(model, managerUserId);
        }

        public async Task<bool> Delete(int id)
        {
            return await _groupSelectedDisciplinesRepository.Delete(id);
        }
    }
}
