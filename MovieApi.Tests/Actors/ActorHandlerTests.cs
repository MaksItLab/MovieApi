using MovieApi.Contracts.Actors;
using MovieApi.Core.Common;
using MovieApi.Core.Features.Actors.Create;
using MovieApi.Core.Features.Actors.Delete;
using MovieApi.Core.Features.Actors.GetById;
using MovieApi.Core.Features.Actors.Update;
using MovieApi.Domain.Entities;
using MovieApi.Tests.Fakes;

namespace MovieApi.Tests.Actors
{
    public sealed class ActorHandlerTests
    {
        [Fact]
        public async Task CreateActor_WithValidRequest_CreatesActor()
        {
            var repository = new FakeActorRepository();
            var handler = new CreateActorHandler(repository);
            var request = new CreateActorRequest("Keanu", "Reeves", new DateTime(1964, 9, 2));

            var result = await handler.HandleAsync(request, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal("Keanu", result.Value!.FirstName);
            Assert.Equal("Reeves", result.Value.LastName);
            Assert.Single(repository.Actors);
        }

        [Fact]
        public async Task CreateActor_WithWhitespaceAroundNames_TrimsNames()
        {
            var repository = new FakeActorRepository();
            var handler = new CreateActorHandler(repository);
            var request = new CreateActorRequest("  Keanu  ", "  Reeves  ", null);

            var result = await handler.HandleAsync(request, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal("Keanu", result.Value!.FirstName);
            Assert.Equal("Reeves", result.Value.LastName);
        }

        [Fact]
        public async Task CreateActor_WithEmptyFirstName_ReturnsValidationError()
        {
            var repository = new FakeActorRepository();
            var handler = new CreateActorHandler(repository);
            var request = new CreateActorRequest("", "Reeves", null);

            var result = await handler.HandleAsync(request, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal(OperationErrorType.Validation, result.ErrorType);
            Assert.Empty(repository.Actors);
        }

        [Fact]
        public async Task CreateActor_WithEmptyLastName_ReturnsValidationError()
        {
            var repository = new FakeActorRepository();
            var handler = new CreateActorHandler(repository);
            var request = new CreateActorRequest("Keanu", "   ", null);

            var result = await handler.HandleAsync(request, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal(OperationErrorType.Validation, result.ErrorType);
            Assert.Empty(repository.Actors);
        }

        [Fact]
        public async Task CreateActor_WithFutureBirthDate_ReturnsValidationError()
        {
            var repository = new FakeActorRepository();
            var handler = new CreateActorHandler(repository);
            var request = new CreateActorRequest("Keanu", "Reeves", DateTime.UtcNow.Date.AddDays(1));

            var result = await handler.HandleAsync(request, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal(OperationErrorType.Validation, result.ErrorType);
            Assert.Empty(repository.Actors);
        }

        [Fact]
        public async Task GetActorById_WhenActorExists_ReturnsActor()
        {
            var repository = new FakeActorRepository();
            var actor = CreateActor();
            repository.AddExisting(actor);
            var handler = new GetActorByIdHandler(repository);

            var result = await handler.HandleAsync(actor.Id, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(actor.Id, result.Value!.Id);
        }

        [Fact]
        public async Task GetActorById_WhenActorDoesNotExist_ReturnsNotFound()
        {
            var handler = new GetActorByIdHandler(new FakeActorRepository());

            var result = await handler.HandleAsync(Guid.NewGuid(), CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal(OperationErrorType.NotFound, result.ErrorType);
        }

        [Fact]
        public async Task UpdateActor_WhenActorExists_UpdatesActor()
        {
            var repository = new FakeActorRepository();
            var actor = CreateActor();
            repository.AddExisting(actor);
            var handler = new UpdateActorHandler(repository);
            var request = new UpdateActorRequest("Carrie-Anne", "Moss", new DateTime(1967, 8, 21));

            var result = await handler.HandleAsync(actor.Id, request, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal("Carrie-Anne", result.Value!.FirstName);
            Assert.Equal("Moss", result.Value.LastName);
        }

        [Fact]
        public async Task DeleteActor_WhenActorExists_RemovesActor()
        {
            var repository = new FakeActorRepository();
            var actor = CreateActor();
            repository.AddExisting(actor);
            var handler = new DeleteActorHandler(repository);

            var result = await handler.HandleAsync(actor.Id, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Empty(repository.Actors);
        }

        [Fact]
        public async Task DeleteActor_WhenActorUsedByMovie_ReturnsConflict()
        {
            var repository = new FakeActorRepository();
            var actor = CreateActor();
            repository.AddExisting(actor);
            repository.MarkAsUsedByMovie(actor.Id);
            var handler = new DeleteActorHandler(repository);

            var result = await handler.HandleAsync(actor.Id, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal(OperationErrorType.Conflict, result.ErrorType);
            Assert.Single(repository.Actors);
        }

        private static Actor CreateActor()
        {
            var now = DateTime.UtcNow;

            return new Actor
            {
                Id = Guid.NewGuid(),
                FirstName = "Keanu",
                LastName = "Reeves",
                BirthDate = new DateTime(1964, 9, 2),
                CreatedAt = now,
                UpdatedAt = now
            };
        }
    }
}
