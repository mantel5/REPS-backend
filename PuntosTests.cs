using Xunit;

namespace REPS_backend.Tests
{
    public class PuntosTests
    {
        [Fact]
        public void CalculoPuntos_ValorLimite_DeberiaSerCorrecto()
        {
            // Arrange
            var puntosIniciales = 100;
            var puntosGanados = 50;
            // Act
            var total = puntosIniciales + puntosGanados;
            // Assert
            Assert.Equal(150, total);
        }

        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(10, 5, 15)]
        [InlineData(100, -10, 90)]
        public void CalculoPuntos_VariosCasos_DeberiaSerCorrecto(int inicial, int ganados, int esperado)
        {
            // Arrange
            // Act
            var total = inicial + ganados;
            // Assert
            Assert.Equal(esperado, total);
        }
    }
}
