namespace AcApi.Infrastructure.Exception
{
    public class BusinessRuleExpcetion : System.Exception
    {
        public BusinessRuleExpcetion() : base() { }
        public BusinessRuleExpcetion(string message) : base(message) { }
        public BusinessRuleExpcetion(string message, System.Exception e) : base(message, e) { }
    }
}
