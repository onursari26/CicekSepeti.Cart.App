namespace CicekSepeti.Api.Validations
{
    public struct ValidationMessage
    {
        public const string NotEmpty = "{PropertyName} cannot be empty";
        public const string MaxLength = "{PropertyName} must not exceed {MaxLength} characters.";
        public const string NotZero = "{PropertyName} cannot be less than zero";
        public const string InEnum = "{PropertyName}  must be in enum values.";
    }
}
