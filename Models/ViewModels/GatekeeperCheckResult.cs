namespace QLedger.Models.ViewModels
{
    public class GatekeeperCheckResult
    {
        public bool IsEligible { get; set; } = true;
        public string MachineStatus { get; set; } = "OK"; // "OK", "NG", "NO_DATA"
        public string MachineMessage { get; set; } = string.Empty;
        public string NrsStatus { get; set; } = "OK"; // "OK", "HOLD"
        public string NrsMessage { get; set; } = string.Empty;
        public string OverallMessage { get; set; } = string.Empty;
    }
}

