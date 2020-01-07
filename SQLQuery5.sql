SELECT  Employee.Id AS 'Id', FirstName, 
LastName, Department.Name AS 'DeptName', isSupervisor ,  Computer.Make, Computer.Manufacturer
FROM Employee  
 JOIN Department ON DepartmentId = Department.Id Join ComputerEmployee ON Employee.Id = EmployeeId JOIN Computer ON Computer.Id = ComputerId

