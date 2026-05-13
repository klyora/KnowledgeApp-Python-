using KnowledgeApp.DataAccess.Repositories;
using KnowledgeApp.Core.Models;

namespace KnowledgeApp.Application.Services
{
    public class GroupPeriodStatusService
    {
        private readonly GroupPeriodStatusRepository _groupPeriodStatusRepository;

        public GroupPeriodStatusService(GroupPeriodStatusRepository groupPeriodStatusRepository)
        {
            _groupPeriodStatusRepository = groupPeriodStatusRepository;
        }

        public async Task<List<GroupPeriodStatusModel>> GetAll()
        {
            return await _groupPeriodStatusRepository.GetAll();
        }

        public async Task<GroupPeriodStatusModel> GetById(int id)
        {
            return await _groupPeriodStatusRepository.GetById(id);
        }

        public async Task<GroupPeriodStatusModel?> GetByGroupAndSemester(int groupId, int semesterId)
        {
            return await _groupPeriodStatusRepository.GetByGroupAndSemester(groupId, semesterId);
        }

        public async Task<List<GroupPeriodStatusModel>> GetByGroupId(int groupId)
        {
            return await _groupPeriodStatusRepository.GetByGroupId(groupId);
        }

        public async Task<GroupPeriodStatusModel> Create(GroupPeriodStatusModel model)
        {
            return await _groupPeriodStatusRepository.Create(model);
        }

        public async Task<GroupPeriodStatusModel> Update(GroupPeriodStatusModel model)
        {
            return await _groupPeriodStatusRepository.Update(model);
        }

        // Установить статус (создать или обновить)
        public async Task<GroupPeriodStatusModel> SetStatus(int groupId, int semesterId, bool status, int updatedBy)
        {
            var model = new GroupPeriodStatusModel(groupId, semesterId, status, updatedBy);
            return await _groupPeriodStatusRepository.Upsert(model);
        }

        public async Task<bool> Delete(int id)
        {
            return await _groupPeriodStatusRepository.Delete(id);
        }
    }
}
