using System.Text.Json;
using System.Text.Json.Serialization;
using FSAuth.Core;
using FSDataUtil.Core;

namespace CM.Shared {

    // UserAuth_DTO.Roles is a List<IRoleDTO> and UserRoleDTO.Permissions a List<IPermissionDTO>.
    // System.Text.Json cannot create an instance for an interface on its own, so without these
    // two converters the localStorage snapshot AppStateManager saves at sign-in cannot be read
    // back, and a server restart would sign every active user out.
    //
    // Each interface has exactly one implementation in FSAuth.Core (UserRoleDTO,
    // UserPermissionDTO), so mapping straight to it is safe. Registered only on this app's
    // Blazored.LocalStorage options in Program.cs.
    //
    // Same converters as ETS.Shared's AuthDtoJsonConverters.cs. They are not app-specific and
    // would be better owned by FSAuth.Core, next to the DTOs they serve.

    public sealed class RoleDTOJsonConverter : JsonConverter<IRoleDTO> {
        public override IRoleDTO? Read(ref Utf8JsonReader toReader, Type toTypeToConvert, JsonSerializerOptions toOptions) =>
            JsonSerializer.Deserialize<UserRoleDTO>(ref toReader, toOptions);

        public override void Write(Utf8JsonWriter toWriter, IRoleDTO toValue, JsonSerializerOptions toOptions) =>
            JsonSerializer.Serialize(toWriter, toValue, toValue.GetType(), toOptions);
    }

    public sealed class PermissionDTOJsonConverter : JsonConverter<IPermissionDTO> {
        public override IPermissionDTO? Read(ref Utf8JsonReader toReader, Type toTypeToConvert, JsonSerializerOptions toOptions) =>
            JsonSerializer.Deserialize<UserPermissionDTO>(ref toReader, toOptions);

        public override void Write(Utf8JsonWriter toWriter, IPermissionDTO toValue, JsonSerializerOptions toOptions) =>
            JsonSerializer.Serialize(toWriter, toValue, toValue.GetType(), toOptions);
    }
}
