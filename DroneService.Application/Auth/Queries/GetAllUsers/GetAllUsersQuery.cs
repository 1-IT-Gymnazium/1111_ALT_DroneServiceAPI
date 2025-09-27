using DroneService.Application.Contracts.Auth;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneService.Application.Auth.Queries.GetAllUsers;

public class GetAllUsersQuery : IRequest<List<AppUserDto>> { }
