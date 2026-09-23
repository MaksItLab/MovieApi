using Microsoft.Extensions.DependencyInjection;
using MovieApi.Core.Features.Actors.Create;
using MovieApi.Core.Features.Actors.Delete;
using MovieApi.Core.Features.Actors.GetById;
using MovieApi.Core.Features.Actors.GetList;
using MovieApi.Core.Features.Actors.Update;
using MovieApi.Core.Features.Auth;
using MovieApi.Core.Features.Genres.Create;
using MovieApi.Core.Features.Genres.Delete;
using MovieApi.Core.Features.Genres.GetById;
using MovieApi.Core.Features.Genres.GetList;
using MovieApi.Core.Features.Genres.Update;
using MovieApi.Core.Features.Movies.Create;
using MovieApi.Core.Features.Movies.Delete;
using MovieApi.Core.Features.Movies.DeletePoster;
using MovieApi.Core.Features.Movies.DeleteVideo;
using MovieApi.Core.Features.Movies.GetById;
using MovieApi.Core.Features.Movies.GetList;
using MovieApi.Core.Features.Movies.GetPosterAccess;
using MovieApi.Core.Features.Movies.GetVideoAccess;
using MovieApi.Core.Features.Movies.ReplacePoster;
using MovieApi.Core.Features.Movies.Update;
using MovieApi.Core.Features.Movies.UploadPoster;
using MovieApi.Core.Features.Movies.UploadVideo;
using MovieApi.Core.Features.Reviews.Create;
using MovieApi.Core.Features.Reviews.Delete;
using MovieApi.Core.Features.Reviews.GetById;
using MovieApi.Core.Features.Reviews.GetList;
using MovieApi.Core.Features.Reviews.Update;
using MovieApi.Core.Features.Users;
using MovieApi.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApi.Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            services.AddScoped<IPasswordHashService, PasswordHashService>();

            return services;
        }

        public static IServiceCollection AddHandlers(this IServiceCollection services)
        {
            services.AddScoped<GetMovieByIdHandler>();
            services.AddScoped<CreateMovieHandler>();
            services.AddScoped<GetMoviesHandler>();
            services.AddScoped<UpdateMovieHandler>();
            services.AddScoped<DeleteMovieHandler>();

            services.AddScoped<GetGenreByIdHandler>();
            services.AddScoped<CreateGenreHandler>();
            services.AddScoped<GetGenresHandler>();
            services.AddScoped<UpdateGenreHandler>();
            services.AddScoped<DeleteGenreHandler>();

            services.AddScoped<GetActorByIdHandler>();
            services.AddScoped<CreateActorHandler>();
            services.AddScoped<GetActorsHandler>();
            services.AddScoped<UpdateActorHandler>();
            services.AddScoped<DeleteActorHandler>();

            services.AddScoped<GetReviewByIdHandler>();
            services.AddScoped<GetMovieReviewsHandler>();
            services.AddScoped<CreateReviewHandler>();
            services.AddScoped<UpdateReviewHandler>();
            services.AddScoped<DeleteReviewHandler>();

            services.AddScoped<RegisterHandler>();
            services.AddScoped<LoginHandler>();

            services.AddScoped<GetMyProfileHandler>();

            services.AddScoped<UploadMoviePosterHandler>();
            services.AddScoped<GetMoviePosterAccessHandler>();
            services.AddScoped<ReplaceMoviePosterHandler>();
            services.AddScoped<DeleteMoviePosterHandler>();

            services.AddScoped<UploadMovieVideoHandler>();
            services.AddScoped<GetMovieVideoAccessHandler>();
            services.AddScoped<DeleteMovieVideoHandler>();

            return services;
        }
    }
}
