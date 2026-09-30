using JobApplication.Domain.Entities;

namespace JobApplication.Application.Common.Interfaces;

public interface IApplicationRepository
{
    Task InsertAsync(JobCandidateApplication application);
    Task<JobCandidateApplication?> GetByIdAsync(int id);
    Task<bool> ExistsAsync(int jobId, int candidateId);
    Task<Candidate?> GetCandidateByAppUserIdAsync(string appUserId);
    void Update(JobCandidateApplication application);
    Task SaveChangesAsync();
}
