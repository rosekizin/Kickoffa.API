using Kickoffa.API.Contracts.Checklist.Components.Response;
using Newtonsoft.Json;

namespace Kickoffa.API.Contracts.UnitTests.Checklist.Components.Response
{
	public class ComponentResponseTests
	{
		// Since ComponentResponse is abstract, we'll use a concrete implementation for testing
		private sealed record TestComponentResponse : ComponentResponse
		{
			public TestComponentResponse()
			{
				Type = "test";
			}
		}

		[Fact]
		public void ComponentResponse_WithRequiredProperties_ShouldCreateSuccessfully()
		{
			// Arrange
			var id = 1L;
			var sectionId = 2L;
			var title = "Test Component";
			var type = "test";
			var isRequired = true;
			var order = 1;
			var createdDate = DateTime.UtcNow;
			var lastUpdatedDate = DateTime.UtcNow;

			// Act
			var response = new TestComponentResponse
			{
				Id = id,
				SectionId = sectionId,
				Title = title,
				Type = type,
				IsRequired = isRequired,
				Order = order,
				CreatedDateUtc = createdDate,
				LastUpdatedDateUtc = lastUpdatedDate
			};

			// Assert
			Assert.Equal(id, response.Id);
			Assert.Equal(sectionId, response.SectionId);
			Assert.Equal(title, response.Title);
			Assert.Equal(type, response.Type);
			Assert.Equal(isRequired, response.IsRequired);
			Assert.Equal(order, response.Order);
			Assert.Equal(createdDate, response.CreatedDateUtc);
			Assert.Equal(lastUpdatedDate, response.LastUpdatedDateUtc);
			Assert.Null(response.Description);
			Assert.Null(response.Status);
		}

		[Fact]
		public void ComponentResponse_WithAllProperties_ShouldSetCorrectly()
		{
			// Arrange
			var id = 1L;
			var sectionId = 2L;
			var title = "Test Component";
			var description = "Test description";
			var type = "test";
			var isRequired = false;
			var order = 3;
			var status = CreateComponentStatusResponse();
			var createdDate = DateTime.UtcNow;
			var lastUpdatedDate = DateTime.UtcNow;

			// Act
			var response = new TestComponentResponse
			{
				Id = id,
				SectionId = sectionId,
				Title = title,
				Description = description,
				Type = type,
				IsRequired = isRequired,
				Order = order,
				Status = status,
				CreatedDateUtc = createdDate,
				LastUpdatedDateUtc = lastUpdatedDate
			};

			// Assert
			Assert.Equal(id, response.Id);
			Assert.Equal(sectionId, response.SectionId);
			Assert.Equal(title, response.Title);
			Assert.Equal(description, response.Description);
			Assert.Equal(type, response.Type);
			Assert.Equal(isRequired, response.IsRequired);
			Assert.Equal(order, response.Order);
			Assert.Equal(status, response.Status);
			Assert.Equal(createdDate, response.CreatedDateUtc);
			Assert.Equal(lastUpdatedDate, response.LastUpdatedDateUtc);
		}

		[Fact]
		public void ComponentResponse_AsRecord_ShouldSupportEquality()
		{
			// Arrange
			var createdDate = DateTime.UtcNow;
			var lastUpdatedDate = DateTime.UtcNow;

			var response1 = new TestComponentResponse
			{
				Id = 1L,
				SectionId = 2L,
				Title = "Test Component",
				Type = "test",
				IsRequired = true,
				Order = 1,
				CreatedDateUtc = createdDate,
				LastUpdatedDateUtc = lastUpdatedDate
			};

			var response2 = new TestComponentResponse
			{
				Id = 1L,
				SectionId = 2L,
				Title = "Test Component",
				Type = "test",
				IsRequired = true,
				Order = 1,
				CreatedDateUtc = createdDate,
				LastUpdatedDateUtc = lastUpdatedDate
			};

			// Act & Assert
			Assert.Equal(response1, response2);
			Assert.True(response1 == response2);
			Assert.False(response1 != response2);
		}

		[Fact]
		public void ComponentResponse_JsonSerialization_ShouldUseCorrectPropertyNames()
		{
			// Arrange
			var response = new TestComponentResponse
			{
				Id = 1L,
				SectionId = 2L,
				Title = "Test Component",
				Description = "Test description",
				Type = "test",
				IsRequired = true,
				Order = 1,
				CreatedDateUtc = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc),
				LastUpdatedDateUtc = new DateTime(2024, 1, 2, 12, 0, 0, DateTimeKind.Utc)
			};

			// Act
			var json = JsonConvert.SerializeObject(response);

			// Assert
			Assert.Contains("\"id\":", json);
			Assert.Contains("\"sectionId\":", json);
			Assert.Contains("\"title\":", json);
			Assert.Contains("\"description\":", json);
			Assert.Contains("\"type\":", json);
			Assert.Contains("\"isRequired\":", json);
			Assert.Contains("\"order\":", json);
			Assert.Contains("\"createdDateUtc\":", json);
			Assert.Contains("\"lastUpdatedDateUtc\":", json);
		}

		#region Helper Methods

		private static ComponentStatusResponse CreateComponentStatusResponse()
		{
			return new ComponentStatusResponse
			{
				Id = 1L,
				ComponentId = 1L,
				IsCompleted = true,
				CompletedAt = DateTime.UtcNow,
				CreatedDateUtc = DateTime.UtcNow,
				LastUpdatedDateUtc = DateTime.UtcNow
			};
		}

		#endregion
	}
}