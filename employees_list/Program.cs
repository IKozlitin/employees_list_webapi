using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace employees_list
{
    public class Program
    {
        /*static List<Employee> employees = new List<Employee>
        {
            new Employee { Id = Guid.NewGuid().ToString(), Name="Ivanov Ivan", Department="Development", Phone=79208887766},
            new Employee { Id = Guid.NewGuid().ToString(), Name="Petrov Petr", Department="Design", Phone=79208885544},
            new Employee { Id = Guid.NewGuid().ToString(), Name="Magomedov Magomed", Department="Test", Phone=79208883322}
        };*/
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));//Подключение к БД
            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();//исполбзование директории статических файлов wwwroot

            //Получение всех сотрудников
            app.MapGet("/api/employees", GetAllEmployees);
            //Получение сотрудника по id
            app.MapGet("/api/employees/{id:guid}", GetEmployee);
            //Удаление сотрудника
            app.MapDelete("/api/employees/{id:guid}", DeleteEmployee);
            //Добавление сотрудника
            app.MapPost("/api/employees", CreateEmployee);
            //Изменение сотрудника
            app.MapPut("/api/employees", UpdateEmployee);

            app.Run();
        }
        static IResult GetAllEmployees(ApplicationContext db)
        {
            //сериализуем список пользователей в json, отправляем пользователю
            return Results.Json(db.Employees.ToList());
        }

        static IResult GetEmployee(string id, ApplicationContext db)
        {
            //ищем пользователя по id
            Employee employee = db.Employees.FirstOrDefault(e => e.Id == id);
            //если найден
            if (employee != null)
            {
                return Results.Json(employee);
            }
            else
            {
                //если не найден - устанавливаем статус и текст ошибки                
                return Results.NotFound(new { message = "Сотрудник не найден" });
            }
        }
        static IResult DeleteEmployee(string id, ApplicationContext db)
        {
            //ищем сотрудника по id
            Employee employee = db.Employees.FirstOrDefault(e => e.Id == id);
            //если найден
            if (employee != null)
            {
                db.Employees.Remove(employee);//удаляем сотрудника
                db.SaveChanges();
                return Results.Json(employee);
            }
            else
            {
                //если не найден - устанавливаем статус и текст ошибки
                return Results.NotFound(new { message = "Сотрудник не найден" });
            }
        }

        static IResult CreateEmployee(Employee employee, ApplicationContext db)
        {
            try
            {
                //извлекаем из запроса отправленные от клиента данные, десериализуем в User
                employee.Id = Guid.NewGuid().ToString();
                db.Employees.Add(employee);
                db.SaveChanges();
                return Results.Json(employee);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        }
        static IResult UpdateEmployee(Employee employeeData, ApplicationContext db)
        {
            try
            {
                //находим "оригинального" пользователя по id
                var employee = db.Employees.FirstOrDefault(e => e.Id == employeeData.Id);
                if (employee != null)
                {
                    employee.Name = employeeData.Name;
                    employee.Department = employeeData.Department;
                    employee.Phone = employeeData.Phone;
                    db.SaveChanges();
                    return Results.Json(employee);
                }
                else
                {
                    //если не найден - устанавливаем статус и текст ошибки(404 - NotFound)
                    return Results.NotFound(new { message = "Сотрудник не найден" });
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
