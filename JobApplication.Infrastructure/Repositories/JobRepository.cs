using JobApplication.Application.Common.Interfaces;
using JobApplication.Domain.Entities;
using JobApplication.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace JobApplication.Infrastructure.Repositories;

public class JobRepository : IJobRepository
{
    private readonly ApplicationDbContext _context;

    public JobRepository(ApplicationDbContext context) => _context = context;

    public async Task InsertAsync(Job job) => await _context.Jobs.AddAsync(job);

    public async Task<Job?> GetByIdAsync(int id) => await _context.Jobs.FirstOrDefaultAsync(j => j.Id == id);

    public async Task<List<JobCandidateApplication>> GetApplicationsByJobIdAsync(int jobId) =>
        await _context.JobCandidateApplications
            .Where(a => a.JobId == jobId)
            .ToListAsync();

    public void Update(Job job) => _context.Jobs.Update(job);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}
