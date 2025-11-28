namespace ApiGateway.Models
{
    public class AccessControlSettings
    {
        public List<string> WhiteList { get; set; } = new(); // 混合值：IP、IP段("192.168.1.0/24","fe80::/10")、机器名
        public List<string> BlackList { get; set; } = new(); // 混合值：IP、IP段、机器名
        public string RedirectUrl { get; set; } = string.Empty; // 拦截后跳转地址
    }


    // 解析后的分组（新增通配符IP规则）
    public class ParsedAccessList
    {
        public List<string> ExactIps { get; set; } = new(); // 精确IP（如192.168.1.100）
        public List<string> CidrRanges { get; set; } = new(); // CIDR段（如192.168.1.0/24）
        public List<string> WildcardIps { get; set; } = new(); // 通配符IP（如10.114.*、10.186.16*）
        public List<string> ExactMachineNames { get; set; } = new(); // 精确机器名（如SERVER-01）
        public List<string> WildcardMachineNames { get; set; } = new(); // 模糊机器名（如itstg*、*itdev*）
    }
}
