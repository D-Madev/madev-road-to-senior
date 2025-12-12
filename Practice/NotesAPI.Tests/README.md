## 🛡️ Estándar Senior: Cobertura de Tests (Controller y Service)

### I. Tests Unitarios para el Controller (`NotesController`)

El objetivo es probar que el **Controller** (la capa HTTP) maneja correctamente las solicitudes y devuelve el **código HTTP** esperado, basándose en la respuesta que le da el *Service* **mockeado**.

| Método HTTP | Escenario (Resultado Esperado) | Código HTTP | Estado |
| :--- | :--- | :--- | :--- |
| **`GET /notes`** | **Happy Path:** Devuelve una lista de notas. | `200 OK` | ✅ |
| **`GET /notes`** | **Lista Vacía:** Devuelve una lista vacía (no un 404). | `200 OK` | ✅ |
| **`GET /notes/{id}`** | **Happy Path:** Devuelve la nota solicitada. | `200 OK` | ✅ |
| **`GET /notes/{id}`** | **No Encontrado:** La nota no existe. | `404 Not Found` | ✅ |
| **`POST /notes`** | **Creación Exitosa:** Se crea la nota. | `201 Created` | ✅ |
| **`POST /notes`** | **Fallo de Service:** El Service devuelve `null` (Ej. `newNote` es `null`). | `404 Not Found` | ⚠️ PENDIENTE |
| **`PUT /notes/{id}`** | **Actualización Exitosa:** Se actualiza la nota. | `204 No Content` | ✅ |
| **`PUT /notes/{id}`** | **Error de Cliente:** Ids de ruta y cuerpo no coinciden. | `400 Bad Request` | ✅ |
| **`PUT /notes/{id}`** | **No Encontrado:** La nota a actualizar no existe. | `404 Not Found` | ✅ |
| **`DELETE /notes/{id}`** | **Eliminación Exitosa:** Se elimina la nota. | `204 No Content` | ✅ |
| **`DELETE /notes/{id}`** | **No Encontrado:** La nota a eliminar no existe. | `404 Not Found` | ✅ |

---

### II. Integration Tests para el Service (`NotesService`)

El objetivo es probar la **lógica de negocio** y la **interacción con la DB** usando una **Base de Datos en Memoria** (`UseInMemoryDatabase`) para asegurar que el código de EF Core y las validaciones de negocio funcionan. 

| Operación | Escenario (Lógica de Negocio) | Resultado Esperado | Estado |
| :--- | :--- | :--- | :--- |
| **`GetAllAsync`** | **Happy Path:** Devuelve todas las notas. | Retorna `List<Note>` con todos los elementos de la DB. | ✅ |
| **`GetByIdAsync`** | **Happy Path:** ID válido y existente. | Retorna el objeto `Note` encontrado. | ✅ |
| **`GetByIdAsync`** | **Validación (Edge Case):** ID $\le 0$ (Inválido). | Retorna `null`. (Controla la validación: `if (id <= 0)`) | ✅ |
| **`GetByIdAsync`** | **No Encontrado:** ID válido pero inexistente. | Retorna `null`. | ✅ |
| **`CreateAsync`** | **Happy Path:** Nota válida. | Retorna la `Note` creada y verifica que se insertó en la DB. | ✅ |
| **`CreateAsync`** | **Validación (Edge Case):** `newNote` es `null`. | Retorna `null`. (Controla la validación: `if (newNote == null)`) | ✅ |
| **`UpdateAsync`** | **Happy Path:** Nota existe y se actualiza. | Retorna `true` y verifica que los campos (`Title`/`Content`) cambiaron en la DB. | ✅ |
| **`UpdateAsync`** | **Validación (Edge Case):** ID de ruta $\le 0$. | Retorna `false`. (Controla la validación: `if (id <= 0)`) | ✅ |
| **`UpdateAsync`** | **Validación (Edge Case):** `noteUpdate` es `null`. | Retorna `false`. (Controla la validación: `if (noteUpdate == null)`) | ✅ |
| **`UpdateAsync`** | **No Encontrado:** ID válido pero inexistente. | Retorna `false`. | ✅ |
| **`DeleteAsync`** | **Happy Path:** ID válido y existente. | Retorna `true` y verifica que la nota fue eliminada de la DB. | ✅ |
| **`DeleteAsync`** | **Validación (Edge Case):** ID $\le 0$. | Retorna `false`. (Controla la validación: `if (id <= 0)`) | ✅ |
| **`DeleteAsync`** | **No Encontrado:** ID válido pero inexistente. | Retorna `false`. | ✅ |