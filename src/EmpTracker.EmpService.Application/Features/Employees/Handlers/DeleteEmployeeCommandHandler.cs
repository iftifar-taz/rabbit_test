using EmpTracker.EmpService.Application.Exceptions;
using EmpTracker.EmpService.Application.Features.Employees.Commands;
using EmpTracker.EmpService.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EmpTracker.EmpService.Application.Features.Employees.Handlers
{
    public class DeleteEmployeeCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteEmployeeCommandHandler> logger) : IRequestHandler<DeleteEmployeeCommand>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<DeleteEmployeeCommandHandler> _logger = logger;

        public async Task Handle(DeleteEmployeeCommand command, CancellationToken cancellationToken)
        {
            var Employee = await _unitOfWork.EmployeeManager.FirstOrDefaultAsync(x => x.EmployeeId == command.EmployeeId, cancellationToken) ?? throw new BadRequestException("Employee does not exist.");
            _unitOfWork.EmployeeManager.Remove(Employee);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Employee deleted.");
        }
    }
}
