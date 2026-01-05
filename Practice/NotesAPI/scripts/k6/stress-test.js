import BASE_URL from './constants.js';
import { check, sleep } from 'k6';
import http from 'k6/http';

export const options = {
  stages: [
    { duration: '1m', target: 50 },  // Tráfico normal
    { duration: '1m', target: 100 }, // Tráfico pesado
    { duration: '1m', target: 200 }, // Punto de quiebre (Stress)
    { duration: '30s', target: 0 },  // Recuperación
  ],
  thresholds: {
    http_req_failed: ['rate<0.01'], // El test falla si más del 1% de las peticiones dan error
    http_req_duration: ['p(95)<500'], // El 95% de las peticiones deben durar menos de 500ms
  },
};

export default function () {
  // Simulamos un usuario navegando por la lista de notas
  const res = http.get(`${BASE_URL}/notes`);

  check(res, {
    'status is 200': (r) => r.status === 200,
  });

  sleep(0.5); // Los usuarios de stress son agresivos, esperan poco
}