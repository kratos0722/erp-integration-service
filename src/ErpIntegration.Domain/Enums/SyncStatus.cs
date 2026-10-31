namespace ErpIntegration.Domain.Enums;

/// <summary>Lifecycle state of a record with respect to its ERP synchronization.</summary>
public enum SyncStatus
{
    Pending = 0,
    Synced = 1,
    Failed = 2
}
