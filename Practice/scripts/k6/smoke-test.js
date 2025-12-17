import BASE_URL from './constants.js';
import { check, sleep } from 'k6';
import http from 'k6/http';

export const options = {
    vus: 5, // 5 usuarios simulando acciones reales
    duration: '30s',
};

export default function () {
    // --- LOGIN ---
    const loginPayload = JSON.stringify({
        username: 'testuser',
        password: 'password'
    });

    const loginRes = http.post(`${BASE_URL}/auth/login`, loginPayload, {
        headers: { 'Content-Type': 'application/json' },
    });

    // Verificamos si el login fue exitoso y extraemos el token
    check(loginRes, { 'Logged in successfully': (r) => r.status === 200 });
    const token = loginRes.json('token');

    // --- GET NOTES ---
    const getNotesRes = http.get(`${BASE_URL}/notes`, { headers: { 'Content-Type': 'application/json' } });
    check(getNotesRes, { 'Get notes': (r) => r.status === 200 });

    const authHeaders = {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
    };

    // --- CREAR UNA NOTA (Flujo Exitoso) ---
    const notePayload = JSON.stringify({
        title: 'Nota desde k6',
        content: 'Probando el flujo completo'
    });

    const createRes = http.post(`${BASE_URL}/notes`, notePayload, { headers: authHeaders });

    check(createRes, {
        'Note created': (r) => r.status === 201,
        'Response has ID': (r) => r.json('id') !== undefined,
    });

    // --- GET NOTE BY ID ---
    const getNoteByIdBadRes = http.get(`${BASE_URL}/notes/${createRes.json('id')}`, { headers: { 'Content-Type': 'application/json' } });
    check(getNoteByIdBadRes, { 'Get note by ID': (r) => r.status === 401 });

    const getNoteByIdRes = http.get(`${BASE_URL}/notes/${createRes.json('id')}`, { headers: authHeaders });
    check(getNoteByIdRes, { 'Get note by ID': (r) => r.status === 200 });


    // --- SIMULAR ERROR (Bad Request) ---
    // Mandamos una nota sin titulo para forzar el 400
    const badNotePayload = JSON.stringify({ content: 'Sin titulo' });
    const badRes = http.post(`${BASE_URL}/notes`, badNotePayload, { headers: authHeaders });

    check(badRes, {
        'Handled Bad Request': (r) => r.status === 400,
    });

    // --- SIMULAR ERROR (Not Found) ---
    const notFoundRes = http.get(`${BASE_URL}/notes/999999`, { headers: authHeaders });
    check(notFoundRes, {
        'Handled Not Found': (r) => r.status === 404,
    });

    sleep(1); // El usuario "piensa" antes de la siguiente iteración
}