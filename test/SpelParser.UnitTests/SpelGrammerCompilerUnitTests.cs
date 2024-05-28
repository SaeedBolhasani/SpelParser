using FluentAssertions;

namespace SpelParser.UnitTests;

public enum EmployeeType : byte
{
    Manager = 1,
    Employee = 2
}

public class EmployeeModel
{
    public int Age { get; set; }
    public string Name { get; set; }
    public DateOnly BirthDate { get; set; }
    public DateTime RegisterDateTime { get; set; }
    public TimeOnly BedTime { get; set; }
    public TimeSpan WorkingDuration { get; set; }
    public decimal AccountBalance { get; set; }
    public EmployeeType EmployeeType { get; set; }
}

public class SpelGrammerCompilerUnitTests
{

    [Fact]
    public void CreateFunc_QueryWithGreaterOperator_ResultSetShouldBeContainOnlyEligibleItems()
    {
        const int age = 45;

        var compiler = new SpelGrammerCompiler<EmployeeModel>();


        var input = $"age > {age} ";
        var query = compiler.CreateFunc(input);

        var result = _models.Where(query.Compile()).ToArray();

        result.Should().AllSatisfy(i => i.Age.Should().BeGreaterThan(age));
    }

    [Fact]
    public void CreateFunc_QueryWithGreaterOrEqualOperator_ResultSetShouldBeContainOnlyEligibleItems()
    {
        const int age = 50;

        var compiler = new SpelGrammerCompiler<EmployeeModel>();

        var input = $"age >= {age} ";
        var query = compiler.CreateFunc(input);

        var result = _models.Where(query.Compile()).ToArray();

        result.Should().AllSatisfy(i => i.Age.Should().BeGreaterThanOrEqualTo(age));
    }

    [Fact]
    public void CreateFunc_QueryWithLessThanOperator_ResultSetShouldBeContainOnlyEligibleItems()
    {
        const int accountBalance = 200;

        var compiler = new SpelGrammerCompiler<EmployeeModel>();

        var input = $"accountBalance < {accountBalance} ";
        var query = compiler.CreateFunc(input);

        var result = _models.Where(query.Compile()).ToArray();

        result.Should().AllSatisfy(i => i.AccountBalance.Should().BeLessThan(accountBalance));
        result.Should().HaveCount(1);
    }

    [Fact]
    public void CreateFunc_QueryWithLessThanOrEqualOperator_ResultSetShouldBeContainOnlyEligibleItems()
    {
        const int accountBalance = 200;

        var compiler = new SpelGrammerCompiler<EmployeeModel>();

        var input = $"accountBalance <= {accountBalance} ";
        var query = compiler.CreateFunc(input);

        var result = _models.Where(query.Compile()).ToArray();

        result.Should().AllSatisfy(i => i.AccountBalance.Should().BeLessThanOrEqualTo(accountBalance));
        result.Should().HaveCount(2);
    }

    [Fact]
    public void CreateFunc_QueryWithEqualOperatorForStringProperty_ResultSetShouldBeContainOnlyEligibleItems()
    {
        const string name = "Ali";

        var compiler = new SpelGrammerCompiler<EmployeeModel>();

        var input = $"name == '{name}' ";
        var query = compiler.CreateFunc(input);

        var result = _models.Where(query.Compile()).ToArray();

        result.Should().AllSatisfy(i => i.Name.Should().BeSameAs(name));
        result.Should().HaveCount(1);
    }

    [Fact]
    public void CreateFunc_QueryWithNotEqualOperatorForStringProperty_ResultSetShouldBeContainOnlyEligibleItems()
    {
        const string name = "Ali";

        var compiler = new SpelGrammerCompiler<EmployeeModel>();

        var input = $"name != '{name}' ";
        var query = compiler.CreateFunc(input);

        var result = _models.Where(query.Compile()).ToArray();

        result.Should().AllSatisfy(i => i.Name.Should().NotBeSameAs(name));
        result.Should().HaveCount(1);
    }

    [Fact]
    public void CreateFunc_QueryWithAndOperator_ResultSetShouldBeContainOnlyEligibleItems()
    {
        const string name = "Ali";
        const int age = 50;

        var compiler = new SpelGrammerCompiler<EmployeeModel>();

        var input = $"name == '{name}' and age >= {age}";
        var query = compiler.CreateFunc(input);

        var result = _models.Where(query.Compile()).ToArray();

        result.Should().OnlyContain(i => i.Name == name && i.Age >= age);

        result.Should().HaveCount(1);
    }

    [Fact]
    public void CreateFunc_QueryWithOrOperator_ResultSetShouldBeContainOnlyEligibleItems()
    {
        const string name = "Ali";
        const int age = 20;

        var compiler = new SpelGrammerCompiler<EmployeeModel>();

        var input = $"name == '{name}' or age >= {age}";
        var query = compiler.CreateFunc(input);

        var result = _models.Where(query.Compile()).ToArray();

        result.Should().OnlyContain(i => i.Name == name || i.Age >= age);

        result.Should().HaveCount(2);
    }

    [Fact]
    public void CreateFunc_QueryWithParantheses_ResultSetShouldBeContainOnlyEligibleItems()
    {
        const string name = "Ali";
        const int age = 20;
        const double accountBalance = 100;

        var compiler = new SpelGrammerCompiler<EmployeeModel>();

        var input = $"((name == '{name}' or age == {age}) and accountBalance > {accountBalance})";
        var query = compiler.CreateFunc(input);

        var result = _models.Where(query.Compile()).ToArray();

        result.Should().HaveCount(1);
    }

    [Fact]
    public void CreateFunc_QueryWithGreaterThanOperatorForDateTimeProperty_ResultSetShouldBeContainOnlyEligibleItems()
    {
        var registerDateTime = DateTime.Now.AddSeconds(-1);

        var compiler = new SpelGrammerCompiler<EmployeeModel>();

        var input = $"registerDateTime > '{registerDateTime}' ";
        var query = compiler.CreateFunc(input);

        var result = _models.Where(query.Compile()).ToArray();

        result.Should().AllSatisfy(i => i.RegisterDateTime.Should().BeAfter(registerDateTime));
        result.Should().HaveCount(2);
    }

    [Fact]
    public void CreateFunc_QueryWithDateOnlyProperty_ResultSetShouldBeContainOnlyEligibleItems()
    {
        var birthDate = DateOnly.Parse("2000-1-1");

        var compiler = new SpelGrammerCompiler<EmployeeModel>();

        var input = $"birthDate > '{birthDate}' ";
        var query = compiler.CreateFunc(input);

        var result = _models.Where(query.Compile()).ToArray();

        result.Should().AllSatisfy(i => i.BirthDate.Should().BeAfter(birthDate));
        result.Should().HaveCount(1);
    }

    [Fact]
    public void CreateFunc_QueryWithTimeOnlyProperty_ResultSetShouldBeContainOnlyEligibleItems()
    {
        var bedTime = TimeOnly.Parse("19:00");

        var compiler = new SpelGrammerCompiler<EmployeeModel>();

        var input = $"bedTime >= '{bedTime}' ";
        var query = compiler.CreateFunc(input);

        var result = _models.Where(query.Compile()).ToArray();

        result.Should().AllSatisfy(i => i.BedTime.Should().BeOnOrAfter(bedTime));
        result.Should().HaveCount(2);
    }

    [Fact]
    public void CreateFunc_QueryWithTimeSpanProperty_ResultSetShouldBeContainOnlyEligibleItems()
    {
        var workingDuration = TimeSpan.Parse("8:00:00");

        var compiler = new SpelGrammerCompiler<EmployeeModel>();

        var input = $"workingDuration == '{workingDuration}' ";
        var query = compiler.CreateFunc(input);

        var result = _models.Where(query.Compile()).ToArray();

        result.Should().AllSatisfy(i => i.WorkingDuration.Should().Be(workingDuration));
        result.Should().HaveCount(2);
    }

    [Fact]
    public void CreateFunc_QueryWithEnum_ResultSetShouldBeContainOnlyEligibleItems()
    {
        var employeeType = EmployeeType.Manager;

        var compiler = new SpelGrammerCompiler<EmployeeModel>();

        var input = $"employeeType == '{employeeType}' ";
        var query = compiler.CreateFunc(input);

        var result = _models.Where(query.Compile()).ToArray();

        result.Should().AllSatisfy(i => i.EmployeeType.Should().Be(employeeType));
        result.Should().HaveCount(1);
    }

    private readonly EmployeeModel[] _models =
     [
         new EmployeeModel
         {
             Age = 50,
             AccountBalance = 100,
             BirthDate = DateOnly.Parse("2020-1-1"),
             RegisterDateTime = DateTime.Now,
             Name = "Ali",
             BedTime = TimeOnly.Parse("19:00"),
             WorkingDuration = TimeSpan.FromHours(8),
             EmployeeType = EmployeeType.Employee
         },
         new EmployeeModel
         {
             Age = 20,
             AccountBalance = 200,
             BirthDate = DateOnly.Parse("2000-1-1"),
             RegisterDateTime = DateTime.Now,
             Name = "Saeed",
             BedTime = TimeOnly.Parse("21:00"),
             WorkingDuration = TimeSpan.FromHours(8),
             EmployeeType = EmployeeType.Manager
         }
      ];

}
