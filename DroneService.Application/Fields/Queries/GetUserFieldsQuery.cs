using DroneService.Application.Contracts.Fields;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneService.Application.Fields.Queries;

public record GetUserFieldsQuery(Guid AuthorId) : IRequest<List<DetailFieldModel>>;
