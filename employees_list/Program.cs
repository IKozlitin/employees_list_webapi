using System.Text.RegularExpressions;

namespace employees_list
{
    public class Program
    {
        static List<Employee> employees = new List<Employee>
        {
            new Employee { Id = Guid.NewGuid().ToString(), Name="Ivanov Ivan", Department="Development", Phone=79208887766},
            new Employee { Id = Guid.NewGuid().ToString(), Name="Petrov Petr", Department="Design", Phone=79208885544},
            new Employee { Id = Guid.NewGuid().ToString(), Name="Magomedov Magomed", Department="Test", Phone=79208883322}
        };
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();//������������� ���������� ����������� ������ wwwroot

            //��������� ���� �����������
            app.MapGet("/api/employees", GetAllEmployees);
            //��������� ���������� �� id
            app.MapGet("/api/employees/{id:guid}", GetEmployee);
            //�������� ����������
            app.MapDelete("/api/employees/{id:guid}", DeleteEmployee);
            //���������� ����������
            app.MapPost("/api/employees", CreateEmployee);
            //��������� ����������
            app.MapPut("/api/employees", UpdateEmployee);

            app.Run();
        }
        static IResult GetAllEmployees()
        {
            //����������� ������ ������������� � json, ���������� ������������
            return Results.Json(employees);
        }

        static IResult GetEmployee(string id)
        {
            //���� ������������ �� id
            Employee employee = employees.FirstOrDefault(e => e.Id == id);
            //���� ������
            if (employee != null)
            {
                return Results.Json(employee);
            }
            else
            {
                //���� �� ������ - ������������� ������ � ����� ������                
                return Results.NotFound(new { message = "��������� �� ������" });
            }
        }
        static IResult DeleteEmployee(string id)
        {
            //���� ���������� �� id
            Employee employee = employees.FirstOrDefault(e => e.Id == id);
            //���� ������
            if (employee != null)
            {
                employees.Remove(employee);//������� ����������
                return Results.Json(employee);
            }
            else
            {
                //���� �� ������ - ������������� ������ � ����� ������
                return Results.NotFound(new { message = "��������� �� ������" });
            }
        }

        static IResult CreateEmployee(Employee employee)
        {
            try
            {
                //��������� �� ������� ������������ �� ������� ������, ������������� � User
                employee.Id = Guid.NewGuid().ToString();
                employees.Add(employee);
                return Results.Json(employee);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        }
        static IResult UpdateEmployee(Employee employeeData)
        {
            try
            {
                //������� "�������������" ������������ �� id
                var employee = employees.FirstOrDefault(e => e.Id == employeeData.Id);
                if (employee != null)
                {
                    employee.Name = employeeData.Name;
                    employee.Department = employeeData.Department;
                    employee.Phone = employeeData.Phone;
                    return Results.Json(employee);
                }
                else
                {
                    //���� �� ������ - ������������� ������ � ����� ������(404 - NotFound)
                    return Results.NotFound(new { message = "��������� �� ������" });
                }

            }
            catch (Exception ex)
            {
                //400 - BadRequest
                return Results.BadRequest(new { message = ex.Message });
            }
        }
    }
}
