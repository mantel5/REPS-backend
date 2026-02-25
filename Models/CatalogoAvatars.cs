namespace REPS_backend.Models
{
    public class AvatarOpcion
    {
        public string Id { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }

    public static class CatalogoAvatars
    {
        public static readonly List<AvatarOpcion> Opciones = new()
        {
            new AvatarOpcion { Id = "avatar_default", Url = "https://res.cloudinary.com/dgtahwqpj/image/upload/v1772035659/unnamed_t93s8g.jpg" },
            new AvatarOpcion { Id = "avatar_robot", Url = "https://res.cloudinary.com/dgtahwqpj/image/upload/v1772035494/unnamed_l44n9h.jpg" },
            new AvatarOpcion { Id = "avatar_gymbro", Url = "https://res.cloudinary.com/dgtahwqpj/image/upload/v1772034939/unnamed_w3uwac.jpg" },
            new AvatarOpcion { Id = "avatar_mujerfit", Url = "https://res.cloudinary.com/dgtahwqpj/image/upload/v1772024580/unnamed_kfdzjz.jpg" },
            new AvatarOpcion { Id = "avatar_hombrefit", Url = "https://res.cloudinary.com/dgtahwqpj/image/upload/v1772024079/unnamed_ojydo4.png" }
        };

        public static AvatarOpcion? Obtener(string id) => Opciones.FirstOrDefault(a => a.Id == id);
    }
}
