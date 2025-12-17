import BASE_URL from './constants.js';
import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    vus: 10,
    duration: '20s',
};

export default function () {
    // ATAQUE DE AUTORIZACIÓN (Sin Token)
    // Intentar borrar algo sin estar logueado
    const unauthorizedRes = http.del(`${BASE_URL}/notes/1`);
    check(unauthorizedRes, {
        'Delete without token returns 401': (r) => r.status === 401,
    });

    // ATAQUE DE DATOS MALFORMADOS (Bad Request)
    const badHeaders = { 'Content-Type': 'application/json' };

    // Enviamos un JSON que no tiene nada que ver con el modelo 'Note'
    const malformedPayload = JSON.stringify({
        "comida_favorita": "pizza",
        "edad": -1
    });

    const badRequestRes = http.post(`${BASE_URL}/notes`, malformedPayload, { headers: badHeaders });
    check(badRequestRes, {
        'Malformed JSON returns 400': (r) => r.status === 400,
    });

    // ATAQUE DE LÍMITES (Payload Gigante)
    // Intentamos enviar un título exageradamente largo para ver si explota la DB o el modelo
    const bigPayload = JSON.stringify({
        title: "A".repeat(10000), // 10k caracteres
        content: "B".repeat(10000)
    });

    const bigRes = http.post(`${BASE_URL}/notes`, bigPayload, { headers: badHeaders });
    // Aquí verás si tu API devuelve 400 (por validación) o 500 (si explota la DB)
    check(bigRes, {
        'Large payload handled': (r) => r.status >= 400,
    });

    // ATAQUE DE RECURSO INEXISTENTE (Not Found)
    const notFoundRes = http.get(`${BASE_URL}/notes/0`); // ID 0 o negativo suele ser inválido
    check(notFoundRes, {
        'Invalid ID returns 404': (r) => r.status === 404,
    });

    sleep(0.5);
}