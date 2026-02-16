using Microsoft.EntityFrameworkCore;
using WindTurbineMonitor.Core.Entities;
using WindTurbineMonitor.Core.Interfaces;
using WindTurbineMonitor.Infrastructure.Data;

namespace WindTurbineMonitor.Infrastructure.Repositories;

/// <summary>
/// Implementasi repository untuk Turbine
/// </summary>
public class TurbineRepository : ITurbineRepository
{
    private readonly AppDbContext _context;

    public TurbineRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Turbine>> GetAllAsync()
    {
        return await _context.Turbines
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<List<Turbine>> GetActiveAsync()
    {
        return await _context.Turbines
            .Where(t => t.IsActive)
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<Turbine?> GetByIdAsync(Guid id)
    {
        return await _context.Turbines
            .Include(t => t.Readings.OrderByDescending(r => r.Timestamp).Take(10))
            .Include(t => t.Events.OrderByDescending(e => e.CreatedAt).Take(50))
            .Include(t => t.ComponentStatuses)
            .Include(t => t.MaintenanceSchedules.Where(m => !m.IsCompleted).OrderBy(m => m.ScheduledDate))
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Turbine> AddAsync(Turbine turbine)
    {
        _context.Turbines.Add(turbine);
        await _context.SaveChangesAsync();
        return turbine;
    }

    public async Task<Turbine> UpdateAsync(Turbine turbine)
    {
        _context.Turbines.Update(turbine);
        await _context.SaveChangesAsync();
        return turbine;
    }

    public async Task DeleteAsync(Guid id)
    {
        var turbine = await GetByIdAsync(id);
        if (turbine != null)
        {
            _context.Turbines.Remove(turbine);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Turbines.AnyAsync(t => t.Id == id);
    }

    public async Task<List<TurbineReading>> GetRecentReadingsAsync(Guid turbineId, int count = 10)
    {
        return await _context.TurbineReadings
            .Where(r => r.TurbineId == turbineId)
            .OrderByDescending(r => r.Timestamp)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<TurbineEvent>> GetRecentEventsAsync(Guid turbineId, int count = 50)
    {
        return await _context.TurbineEvents
            .Where(e => e.TurbineId == turbineId)
            .OrderByDescending(e => e.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<MaintenanceSchedule>> GetUpcomingMaintenanceAsync(Guid turbineId)
    {
        return await _context.MaintenanceSchedules
            .Where(m => m.TurbineId == turbineId && !m.IsCompleted && m.ScheduledDate >= DateTime.UtcNow)
            .OrderBy(m => m.ScheduledDate)
            .ToListAsync();
    }

    public async Task<TurbineEvent> AddEventAsync(TurbineEvent @event)
    {
        _context.TurbineEvents.Add(@event);
        await _context.SaveChangesAsync();
        return @event;
    }

    public async Task<AuditLog> AddAuditLogAsync(AuditLog auditLog)
    {
        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();
        return auditLog;
    }

    public async Task<Turbine?> GetWithLatestReadingAsync(Guid turbineId)
    {
        return await _context.Turbines
            .Include(t => t.Readings
                .OrderByDescending(r => r.Timestamp)
                .Take(1))
            .FirstOrDefaultAsync(t => t.Id == turbineId);
    }

    public async Task<List<TurbineReading>> GetReadingsInDateRangeAsync(
        Guid turbineId,
        DateTime from,
        DateTime to)
    {
        return await _context.TurbineReadings
            .Where(r => r.TurbineId == turbineId && r.Timestamp >= from && r.Timestamp <= to)
            .OrderBy(r => r.Timestamp)
            .ToListAsync();
    }

    public async Task<ComponentStatus?> GetComponentStatusAsync(Guid turbineId, int componentType)
    {
        return await _context.ComponentStatuses
            .Where(cs => cs.TurbineId == turbineId && cs.ComponentType == (Core.Enums.ComponentType)componentType)
            .OrderByDescending(cs => cs.LastUpdated)
            .FirstOrDefaultAsync();
    }
}
