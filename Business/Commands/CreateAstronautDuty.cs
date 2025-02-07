﻿using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Commands
{
    public class CreateAstronautDuty : IRequest<CreateAstronautDutyResult>
    {
        public required string Name { get; set; }

        public required string Rank { get; set; }

        public required string DutyTitle { get; set; }

        public DateTime DutyStartDate { get; set; }
    }

    public class CreateAstronautDutyPreProcessor(StargateContext context) : IRequestPreProcessor<CreateAstronautDuty>
    {
        private readonly StargateContext _context = context;

        public Task Process(CreateAstronautDuty request, CancellationToken cancellationToken)
        {
            var person = _context.People.AsNoTracking().FirstOrDefault(z => z.Name == request.Name) ?? throw new BadHttpRequestException("Bad Request");

            var verifyNoPreviousDuty = _context.AstronautDuties.FirstOrDefault(z => z.DutyTitle == request.DutyTitle && z.DutyStartDate == request.DutyStartDate && z.PersonId == person.Id);

            if (verifyNoPreviousDuty is not null) throw new BadHttpRequestException("Bad Request");

            return Task.CompletedTask;
        }
    }

    public class CreateAstronautDutyHandler(StargateContext context) : IRequestHandler<CreateAstronautDuty, CreateAstronautDutyResult>
    {
        private readonly StargateContext _context = context;

        public async Task<CreateAstronautDutyResult> Handle(CreateAstronautDuty request, CancellationToken cancellationToken)
        {
            var person = _context.People.Where(p => p.Name.Contains(request.Name)).FirstOrDefaultAsync<Person>(cancellationToken: cancellationToken).Result;
            if (person != null)
            {
                var astronautDetail = _context.AstronautDetails.Where(ad => ad.PersonId == person.Id).FirstOrDefaultAsync<AstronautDetail>(cancellationToken: cancellationToken).Result;
                
                if (astronautDetail == null)
                {
                    astronautDetail = new AstronautDetail
                    {
                        PersonId = person.Id,
                        CurrentDutyTitle = request.DutyTitle,
                        CurrentRank = request.Rank,
                        CareerStartDate = request.DutyStartDate.Date
                    };
                    if (request.DutyTitle.Equals("RETIRED", StringComparison.CurrentCultureIgnoreCase))
                    {
                        astronautDetail.CareerEndDate = request.DutyStartDate.Date;
                    }

                    await _context.AstronautDetails.AddAsync(astronautDetail, cancellationToken);

                }
                else
                {
                    astronautDetail.CurrentDutyTitle = request.DutyTitle;
                    astronautDetail.CurrentRank = request.Rank;
                    if (request.DutyTitle.Equals("RETIRED", StringComparison.CurrentCultureIgnoreCase))
                    {
                        astronautDetail.CareerEndDate = request.DutyStartDate.AddDays(-1).Date;
                    }
                    _context.AstronautDetails.Update(astronautDetail);
                }

                var astronautDuty = _context.AstronautDuties.Where(ad => ad.PersonId == person.Id).FirstOrDefaultAsync<AstronautDuty>(cancellationToken: cancellationToken).Result;

                if (astronautDuty != null)
                {
                    astronautDuty.DutyEndDate = request.DutyStartDate.AddDays(-1).Date;
                    _context.AstronautDuties.Update(astronautDuty);
                }

                var newAstronautDuty = new AstronautDuty()
                {
                    PersonId = person.Id,
                    Rank = request.Rank,
                    DutyTitle = request.DutyTitle,
                    DutyStartDate = request.DutyStartDate.Date,
                    DutyEndDate = null
                };

                await _context.AstronautDuties.AddAsync(newAstronautDuty, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return new CreateAstronautDutyResult()
                {
                    Id = newAstronautDuty.Id
                };
            }
            else
            { throw new BadHttpRequestException("Bad Request."); }
        }
    }

    public class CreateAstronautDutyResult : BaseResponse
    {
        public int? Id { get; set; }
    }
}
