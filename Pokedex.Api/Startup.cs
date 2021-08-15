using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pokedex.Service;
using Pokedex.Service.Contracts;
using Pokedex.Service.Contracts.PokeApiDto;
using Pokedex.Service.Contracts.ResponseDto;
using Pokedex.Service.Dependencies;
using Pokedex.Service.Dependencies.Contracts;
using System;
using System.Net.Http.Headers;

namespace Pokedex.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();

            services.AddHttpClient("PokeApi", httpClient =>
            {
                httpClient.BaseAddress = new Uri(Configuration["PokeApi:BaseUri"]);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            services.AddHttpClient("FunTranslationsApi", httpClient =>
            {
                httpClient.BaseAddress = new Uri(Configuration["FunTranslationsApi:BaseUri"]);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            services.AddScoped<IMapper<PokemonSpecies, PokemonDetail>, PokemonDetailResponseMapper>();
            services.AddScoped<IPokeApiService, PokeApiService>();


            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseHttpsRedirection();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("swagger/v1/swagger.json", "Pokedex Api v1");
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
