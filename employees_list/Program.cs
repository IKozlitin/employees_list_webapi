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
            app.UseStaticFiles();//исполбзование директории статических файлов wwwroot

            //ѕолучение всех сотрудников
            app.MapGet("/api/employees", GetAllEmployees);
            //ѕолучение сотрудника по id
            app.MapGet("/api/employees/{id:guid}", GetEmployee);
            //”даление сотрудника
            app.MapDelete("/api/employees/{id:guid}", DeleteEmployee);
            //ƒобавление сотрудника
            app.MapPost("/api/employees", CreateEmployee);
            //»зменение сотрудника
            app.MapPut("/api/employees", UpdateEmployee);

            app.Run();
        }
        static IResult GetAllEmployees()
        {
            //сериализуем список пользователей в json, отправл€ем пользователю
            return Results.Json(employees);
        }

        static IResult GetEmployee(string id)
        {
            //ищем пользовател€ по id
            Employee employee = employees.FirstOrDefault(e => e.Id == id);
            //если найден
            if (employee != null)
            {
                return Results.Json(employee);
            }
            else
            {
                //если не найден - устанавливаем статус и текст ошибки                
                return Results.NotFound(new { message = "—отрудник не найден" });
            }
        }
        static IResult DeleteEmployee(string id)
        {
            //ищем сотрудника по id
            Employee employee = employees.FirstOrDefault(e => e.Id == id);
            //если найден
            if (employee != null)
            {
                employees.Remove(employee);//удал€ем сотрудника
                return Results.Json(employee);
            }
            else
            {
                //если не найден - устанавливаем статус и текст ошибки
                return Results.NotFound(new { message = "—отрудник не найден" });
            }
        }

        static IResult CreateEmployee(Employee employee)
        {
            try
            {
                //извлекаем из запроса отправленные от клиента данные, десериализуем в User
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
                //находим "оригинального" пользовател€ по id
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
                    //если не найден - устанавливаем статус и текст ошибки(404 - NotFound)
                    return Results.NotFound(new { message = "—отрудник не найден" });
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
