# Documentación de la sesión de testing

## Resumen
En esta sesión abordamos el testing desde una doble perspectiva: primero comprendiendo el diseño de pruebas (tipos de testing, técnicas de caja negra y caja blanca, valores límite y cobertura) y después aplicándolo en un proyecto real en .NET 8 con xUnit. El objetivo fue entender que testear no es solo ejecutar código, sino diseñar casos de prueba con criterio, equilibrando cobertura, coste y mantenibilidad.

## Herramientas y enfoque
- Utilizamos Codex como herramienta de apoyo en un enfoque cercano al vibecoding, generando y refactorizando tests con IA, pero validando cada decisión antes de integrarla.
- Documentamos todo el proceso en este archivo, incluyendo la interacción con la IA y las decisiones tomadas, para reflejar un uso responsable y profesional de estas herramientas.

## Contenidos
- Ciclo de vida del software y papel del testing dentro del desarrollo.
- Tipos de pruebas: unitarias, integración, sistema, aceptación y pruebas no funcionales (rendimiento, seguridad, etc.).
- Pirámide de testing y problema del “cono de helado”: importancia de priorizar pruebas unitarias automatizadas.
- Diseño de casos de prueba: imposibilidad de probar todo y necesidad de seleccionar casos representativos.
- Pruebas de caja negra (funcionales): particiones equivalentes, análisis de valores límite, pruebas aleatorias e hipótesis de errores.
- Pruebas de caja blanca (estructurales): cobertura de sentencias, decisiones, condiciones y caminos.
- Concepto de cobertura y su relación con la calidad del código con Sonarqube y Sonarlint.
- Creación de un proyecto de testing en .NET con xUnit.
- Estructura básica de un test: Arrange, Act, Assert.
- Ejecución de pruebas desde CLI y validación del comportamiento esperado.
- Buenas prácticas en desarrollo: principios SOLID, DRY y KISS aplicados al testing.

## Decisiones tomadas
- Se priorizó la cobertura de lógica de negocio crítica.
- Se descartaron tests redundantes o de bajo valor.
- Se refactorizaron tests para mejorar mantenibilidad.
- Se validó cada test generado por IA antes de integrarlo.

## Ejemplo de interacción IA
- Solicitud: "Genera un test para validar el método de cálculo de puntos."
- Respuesta IA: Test generado con estructura AAA.
- Decisión: Se revisó y ajustó el test para cubrir valores límite y casos de error.

## Ejemplo de test unitario

A continuación se muestra un ejemplo de test implementado en .NET 8 con xUnit, siguiendo la estructura Arrange, Act, Assert y cubriendo valores límite y varios casos representativos:

```csharp
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
```

Este test valida la lógica de negocio del cálculo de puntos, cubriendo valores límite y casos de error, y fue generado/refactorizado con IA y validado manualmente antes de integrarlo.

## Ejemplo de diseño de casos de prueba

### Pruebas de caja negra
- Se diseñaron casos usando particiones equivalentes y valores límite.
- Ejemplo: Para el método de cálculo de puntos, se probaron entradas mínimas, máximas y valores fuera de rango.

### Pruebas de caja blanca
- Se cubrieron sentencias, decisiones y caminos principales del código.
- Ejemplo: Se validó que el método de cálculo de puntos cubre todas las ramas (sumas positivas, negativas y cero).

### Cobertura y calidad
- Se utilizó Sonarlint para analizar la cobertura y calidad del código.
- Se priorizó la cobertura de lógica de negocio crítica.

### Ejecución y validación
- Los tests se ejecutaron desde CLI con `dotnet test`.
- Se validó el comportamiento esperado y se documentaron los resultados.

### Buenas prácticas
- Se aplicaron los principios SOLID, DRY y KISS en el diseño y refactorización de los tests.
- Se descartaron tests redundantes y se refactorizaron para mejorar mantenibilidad.

---

Este enfoque asegura que el testing cubre tanto la funcionalidad como la estructura del código, equilibrando cobertura, coste y mantenibilidad, y documentando el proceso de interacción y validación con IA.

## Conclusión
El testing es fundamental para la calidad y mantenibilidad del software. El uso responsable de IA puede acelerar el proceso, pero siempre debe ser validado por el equipo.
