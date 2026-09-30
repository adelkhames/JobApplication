using JobApplication.Application.Common.Interfaces;
using JobApplication.Domain.Entities;
using JobApplication.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace JobApplication.Infrastructure.Repositories;

public class ApplicationRepository : IApplicationRepository
{
    private readonly ApplicationDbContext _context;

    public ApplicationRepository(ApplicationDbContext context) => _context = context;

    public async Task InsertAsync(JobCandidateApplication application)
        => await _context.JobCandidateApplications.AddAsync(application);

    public async Task<JobCandidateApplication?> GetByIdAsync(int id)
        => await _context.JobCandidateApplications
            .Include(a => a.Candidate)
            .Include(a => a.Job)
            .FirstOrDefaultAsync(a => a.Id == id);

    public async Task<bool> ExistsAsync(int jobId, int candidateId)
        => await _context.JobCandidateApplications
            .AnyAsync(a => a.JobId == jobId && a.CandidateId == candidateId);

    public async Task<Candidate?> GetCandidateByAppUserIdAsync(string appUserId)
        => await _context.Candidates.FirstOrDefaultAsync(c => c.AppUserId == appUserId);

    public void Update(JobCandidateApplication application) => _context.JobCandidateApplications.Update(application);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}
