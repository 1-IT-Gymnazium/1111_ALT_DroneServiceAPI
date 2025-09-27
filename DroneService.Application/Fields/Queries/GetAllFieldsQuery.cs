using DroneService.Application.Contracts.Fields;
using MediatR;

namespace DroneService.Application.Fields.Queries;

public class GetAllFieldsQuery : IRequest<List<DetailFieldModel>> { }

