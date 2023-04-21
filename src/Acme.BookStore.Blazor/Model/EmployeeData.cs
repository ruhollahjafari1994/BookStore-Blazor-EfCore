using NUglify.JavaScript.Syntax;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acme.BookStore.Blazor.Model
{
    public static class EmployeeData
    {


        public static async Task<List<Employee>> GetEmployeesDataAsync()
        {
            List<Employee> employees = new List<Employee>() {
    new Employee() {Id=1,FirstName="Alex1",LastName="Sanchez1",Salary=100 },
    new Employee() {Id=2,FirstName="Alex2",LastName="Sanchez2",Salary=200 },
    new Employee() {Id=3,FirstName="Alex3",LastName="Sanchez3",Salary=300 },
    new Employee() {Id=4,FirstName="Alex4",LastName="Sanchez4",Salary=400 },
    new Employee() {Id=5,FirstName="Alex5",LastName="Sanchez5",Salary=500 },
    new Employee() {Id=6,FirstName="Alex6",LastName="Sanchez6",Salary=600 },
    new Employee() {Id=7,FirstName="Alex7",LastName="Sanchez7",Salary=700 },
    new Employee() {Id=8,FirstName="Alex8",LastName="Sanchez8",Salary=800 },
    new Employee() {Id=9,FirstName="Alex9",LastName="Sanchez9",Salary=900 },
    new Employee() {Id=10,FirstName="Alex10",LastName="Sanchez10",Salary=1000 },
    new Employee() {Id=11,FirstName="Alex11",LastName="Sanchez11",Salary=1100 },
    new Employee() {Id=12,FirstName="Alex12",LastName="Sanchez12",Salary=1200 },
    new Employee() {Id=13,FirstName="Alex13",LastName="Sanchez13",Salary=1300 },
    new Employee() {Id=14,FirstName="Alex14",LastName="Sanchez14",Salary=1400 },
    new Employee() {Id=15,FirstName="Alex15",LastName="Sanchez15",Salary=1500 },
    new Employee() {Id=16,FirstName="Alex16",LastName="Sanchez16",Salary=1600 },
    new Employee() {Id=17,FirstName="Alex17",LastName="Sanchez17",Salary=1700 },
    new Employee() {Id=18,FirstName="Alex18",LastName="Sanchez18",Salary=1800 },
    new Employee() {Id=19,FirstName="Alex19",LastName="Sanchez19",Salary=1900 },
    new Employee() {Id=20,FirstName="Alex20",LastName="Sanchez20",Salary=2000 },
    new Employee() {Id=21,FirstName="Alex21",LastName="Sanchez21",Salary=2100 },
    new Employee() {Id=22,FirstName="Alex22",LastName="Sanchez22",Salary=2200 },
    new Employee() {Id=23,FirstName="Alex23",LastName="Sanchez23",Salary=2300 },
    new Employee() {Id=24,FirstName="Alex24",LastName="Sanchez24",Salary=2400 },
    new Employee() {Id=25,FirstName="Alex25",LastName="Sanchez25",Salary=2500 },
    new Employee() {Id=26,FirstName="Alex26",LastName="Sanchez26",Salary=2600 },
    new Employee() {Id=27,FirstName="Alex27",LastName="Sanchez27",Salary=2700 },
    new Employee() {Id=28,FirstName="Alex28",LastName="Sanchez28",Salary=2800 },
    new Employee() {Id=29,FirstName="Alex29",LastName="Sanchez29",Salary=2900 }
           };
            return await Task.FromResult(employees);
        }

    }
    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Salary { get; set; }
    }
}
