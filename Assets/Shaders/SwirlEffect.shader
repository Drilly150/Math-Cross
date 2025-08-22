// --- Uniforms (Variables de entrada desde el juego) ---
uniform float time; // El tiempo de juego para animar el remolino.
uniform vec2 resolution; // La resolución de la pantalla (ej: 1920, 1080).
uniform float swirl_strength; // La intensidad del efecto de remolino.
uniform vec3 base_color; // Un color base para el fondo.

// --- Función Principal del Fragment Shader ---
// Se ejecuta para cada píxel en la pantalla.
void fragment() {
    // 1. Normalizar las coordenadas del píxel (de 0.0 a 1.0).
    vec2 uv = gl_FragCoord.xy / resolution.xy;

    // 2. Centrar las coordenadas en (0,0) para que el remolino nazca en el centro.
    vec2 centered_uv = uv - 0.5;

    // 3. Calcular el ángulo y la distancia del píxel al centro.
    float distance = length(centered_uv);
    float angle = atan(centered_uv.y, centered_uv.x);

    // 4. Modificar el ángulo. La torsión es más fuerte cerca del centro
    //    y se anima con el tiempo.
    angle += (1.0 / (distance + 0.1)) * swirl_strength * time;

    // 5. Generar un patrón de color procedural usando las nuevas coordenadas.
    // Usamos el nuevo ángulo y la distancia para crear ondas con funciones seno.
    float color_pattern = sin(distance * 20.0 + angle * 5.0);
    
    // 6. Mezclar el patrón con el color base para el resultado final.
    vec3 final_color = base_color * color_pattern;

    // 7. Asignar el color final al píxel.
    gl_FragColor = vec4(final_color, 1.0);
}