import BASE_URL from './constants.js';
import http from 'k6/http';
import { sleep, check } from 'k6';

export const options = {
    stages: [
        { duration: '30s', target: 20 }, // Sube de 0 a 20 usuarios en 30s
        { duration: '1m', target: 20 },  // Mantente en 20 usuarios por 1 minuto
        { duration: '30s', target: 0 },  // Baja a 0 usuarios
    ]
}

export default function () {
    // Probamos endpoint publico
    const res = http.get(`${BASE_URL}/notes`);

    check(res, {
        'status is 200': (r) => r.status === 200,
        'transition time < 200ms': (r) => r.timings.duration < 200,
    })

    sleep(1);
}