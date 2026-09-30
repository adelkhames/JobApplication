using JobApplication.Domain.Entities;

namespace JobApplication.Application.Common.Interfaces;

public interface IJobRepository
{
    Task InsertAsync(Job job);
    Task<Job?> GetByIdAsync(int id);
    Task<List<JobCandidateApplication>> GetApplicationsByJobIdAsync(int jobId);
    void Update(Job job);
    Task SaveChangesAsync();
}
