﻿using EmpTracker.EmpService.Application.Exceptions;
using EmpTracker.EmpService.Application.Features.Employees.Commands;
using EmpTracker.EmpService.Core.Domain.Entities;
using EmpTracker.EmpService.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EmpTracker.EmpService.Application.Features.Employees.Handlers
{
    public class CreateEmployeeCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateEmployeeCommandHandler> logger) : IRequestHandler<CreateEmployeeCommand>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<CreateEmployeeCommandHandler> _logger = logger;

        public async Task Handle(CreateEmployeeCommand command, CancellationToken cancellationToken)
        {
            var newEmployee = new Employee
            {
                FirstName = command.FirstName,
                LastName = command.LastName,
                Email = command.Email,
                PhoneNumber = command.PhoneNumber,
                DateOfBirth = command.DateOfBirth,
                DateOfJoining = command.DateOfJoining,
                Address = command.Address,
                City = command.City,
                State = command.State,
                Country = command.Country,
                PostalCode = command.PostalCode,
                IsActive = true,
                DepartmentId = command.DepartmentId,
                DesignationId = command.DesignationId,
            };

            await _unitOfWork.EmployeeManager.AddAsync(newEmployee, cancellationToken);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Employee creaded.");
        }
    }
}
