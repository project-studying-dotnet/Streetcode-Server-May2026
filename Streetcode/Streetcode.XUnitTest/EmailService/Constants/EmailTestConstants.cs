namespace Streetcode.XUnitTest.EmailService.Constants;

public static class EmailTestConstants
{
    public const string TestEmail = "test@test.com";
    public const string FromEmail = "from@test.com";
    public const string SmtpServer = "smtp.test.com";
    public const int Port = 465;
    public const string UserName = "user";
    public const string Password = "password";
    public const string Subject = "Subject";
    public const string Content = "Content";

    public const string RabbitMqHostName = "localhost";
    public const int RabbitMqPort = 5672;
    public const string RabbitMqUserName = "guest";
    public const string RabbitMqPassword = "guest";
    public const string RabbitMqQueueName = "email-queue";
}