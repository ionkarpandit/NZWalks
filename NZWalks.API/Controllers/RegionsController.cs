using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    // https://localhost:1234/api/regions
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;

        public RegionsController(NZWalksDbContext dbContext, IRegionRepository regionRepository,
            IMapper mapper)
        {
            this.dbContext = dbContext;
            this.regionRepository = regionRepository;
            this.mapper = mapper;
        }

        // GET ALL Regions
        // GET: https://localhost:portnumber/api/regions
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            //// HardCoded list of regions - temporary code before we connect to the database
            ////var regions = new List<Region>
            ////{
            ////    new Region
            ////    {
            ////        Id = Guid.NewGuid(),
            ////        Name = "Auckland Region",
            ////        Code = "AKL",
            ////        RegionImageUrl = ""
            ////    },
            ////    new Region
            ////    {
            ////        Id = Guid.NewGuid(),
            ////        Name = "Wellington Region",
            ////        Code = "WLG",
            ////        RegionImageUrl = ""
            ////    },
            ////};

            // Get Data from Database - Domain Models
            //var regionsDomain = await dbContext.Regions.ToListAsync();    // Use DbContext directly
            var regionsDomain = await regionRepository.GetAllAsync();       // Use Repository to get data

            // Map Domain Models to DTOs

            // Method 1: Linq Query Syntax
            //var regionsDto = regions.Select(region => new Models.DTO.RegionDto
            //{
            //    Id = region.Id,
            //    Name = region.Name,
            //    Code = region.Code,
            //    RegionImageUrl = region.RegionImageUrl
            //}).ToList();

            // Method 2: Custom Syntax
            //var regionsDto = new List<RegionDto>();
            //foreach (var regionDomain in regionsDomain)
            //{
            //    regionsDto.Add(new RegionDto
            //    {
            //        Id = regionDomain.Id,
            //        Name = regionDomain.Name,
            //        Code = regionDomain.Code,
            //        RegionImageUrl = regionDomain.RegionImageUrl
            //    });
            //}

            // Method 3: AutoMapper
            var regionsDto = mapper.Map<List<RegionDto>>(regionsDomain);

            // Return DTOs
            return Ok(regionsDto);
        }

        // GET SINGLE Region (Get Region By ID)
        // GET: https://localhost:portnumber/api/regions/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            ////var region = dbContext.Regions.Find(id);    // Option 1: Find is another way to get by only "primary key"

            // Get region from database - Domain Model
            //var region = await dbContext.Regions.FirstOrDefaultAsync(r => r.Id == id); // Option 2: Linq Query
            var regionDomain = await regionRepository.GetByIdAsync(id);

            if (regionDomain == null)
            {
                return NotFound(); //404: "Not Found"
            }

            // Map Domain Model to DTO
            // Custom Mapping
            //var regionDto = new RegionDto
            //{
            //    Id = regionDomain.Id,
            //    Name = regionDomain.Name,
            //    Code = regionDomain.Code,
            //    RegionImageUrl = regionDomain.RegionImageUrl
            //};

            // Using AutoMapper
            var regionDto = mapper.Map<RegionDto>(regionDomain);

            // Return DTO back to client
            return Ok(regionDto);
        }

        // POST: To Create New Region
        // POST: https://localhost:portnumber/api/regions
        [HttpPost]
        [ValidateModel]    // Custom Server-side validation
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {
            //// Map DTO to Domain Model
            // Custom Mapping
            //var regionDomainModel = new Region
            //{
            //    Name = addRegionRequestDto.Name,
            //    Code = addRegionRequestDto.Code,
            //    RegionImageUrl = addRegionRequestDto.RegionImageUrl
            //};

            ////if (ModelState.IsValid) // Server-side validation
            ////{
            // AutoMapper Mapping
            var regionDomainModel = mapper.Map<Region>(addRegionRequestDto);

            // Use Domain Model to create Region in Database
            //await dbContext.Regions.AddAsync(regionDomainModel);
            //await dbContext.SaveChangesAsync();    // Save the changes to the database

            regionDomainModel = await regionRepository.CreateAsync(regionDomainModel);

            //// Map Domain Model back to DTO
            // Custom Mapping
            //var regionDto = new RegionDto
            //{
            //    Id = regionDomainModel.Id,
            //    Name = regionDomainModel.Name,
            //    Code = regionDomainModel.Code,
            //    RegionImageUrl = regionDomainModel.RegionImageUrl
            //};

            // AutoMapper Mapping
            var regionDto = mapper.Map<RegionDto>(regionDomainModel);

            return CreatedAtAction(
                nameof(GetById),
                new { id = regionDto.Id },
                regionDto
            );
            ////}
            ////else
            ////{
            ////    return BadRequest(ModelState);
            ////}
        }

        // Update region - PUT
        // PUT: https://localhost:portnumber/api/regions/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {
            // Check if region exists in the database
            ////var regionDomainModel = await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);

            ////if (regionDomainModel == null)
            ////{
            ////    return NotFound(); // 404
            ////}

            ////// Map updated fields from DTO to Domain Model
            ////regionDomainModel.Name = updateRegionRequestDto.Name;
            ////regionDomainModel.Code = updateRegionRequestDto.Code;
            ////regionDomainModel.RegionImageUrl = updateRegionRequestDto.RegionImageUrl;

            ////// Save changes to database
            ////await dbContext.SaveChangesAsync();
            ///

            //// Map DTO to Domain model
            // Custom Mapping
            //var regionDomainModel = new Region
            //{
            //    Code = updateRegionRequestDto.Code,
            //    Name = updateRegionRequestDto.Name,
            //    RegionImageUrl = updateRegionRequestDto.RegionImageUrl
            //};

            var regionDomainModel = mapper.Map<Region>(updateRegionRequestDto);

            regionDomainModel = await regionRepository.UpdateAsync(id, regionDomainModel);

            if (regionDomainModel == null)
            {
                return NotFound(); // 404
            }

            //// Map Domain Model back to DTO
            // Custom Mapping
            //var regionDto = new RegionDto
            //{
            //    Id = regionDomainModel.Id,
            //    Name = regionDomainModel.Name,
            //    Code = regionDomainModel.Code,
            //    RegionImageUrl = regionDomainModel.RegionImageUrl
            //};

            // AutoMapper Mapping
            var regionDto = mapper.Map<RegionDto>(regionDomainModel);

            // Return the updated DTO
            return Ok(regionDto);
        }

        // Delete region - DELETE
        // DELETE: https://localhost:portnumber/api/regions/{id}
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            // Check if region exists in the database
            //var regionDomainModel = await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);
            var regionDomainModel = await regionRepository.DeleteAsync(id);

            if (regionDomainModel == null)
            {
                return NotFound(); // 404
            }

            ////// Remove region from database
            ////dbContext.Regions.Remove(regionDomainModel);    //Remove does not have async version
            ////await dbContext.SaveChangesAsync();

            //// Map Domain Model back to DTO
            // Custom Mapping
            //var regionDto = new RegionDto
            //{
            //    Id = regionDomainModel.Id,
            //    Name = regionDomainModel.Name,
            //    Code = regionDomainModel.Code,
            //    RegionImageUrl = regionDomainModel.RegionImageUrl
            //};

            // AutoMapper Mapping
            var regionDto = mapper.Map<RegionDto>(regionDomainModel);

            // Return the deleted DTO
            return Ok(regionDto);
        }

    }
}