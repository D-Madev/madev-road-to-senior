## 🛡️ Estándar Senior: Cobertura de Tests (Controller y Service)

### I. Tests Unitarios para el Controller (`NotesController`)

El objetivo es probar que el **Controller** (la capa HTTP) maneja correctamente las solicitudes y devuelve el **código HTTP** esperado, basándose en la respuesta que le da el *Service* **mockeado**.

| Método HTTP | Escenario (Resultado Esperado) | Código HTTP | Estado |
| :--- | :--- | :--- | :--- |
| **`GET /notes`** | **Happy Path:** Devuelve una lista de notas. | `200 OK` | ✅ |
| **`GET /notes`** | **Lista Vacía:** Devuelve una lista vacía (no un 404). | `200 OK` | ✅ |
| **`GET /notes/{id}`** | **Happy Path:** Devuelve la nota solicitada. | `200 OK` | ✅ |
| **`GET /notes/{id}`** | **No Encontrado:** La nota no existe. | `404 Not Found` | ✅ |
| **`POST /notes`** | **Creación Exitosa:** Se crea la nota. | `201 Created` | |
| **`PUT /notes/{id}`** | **Actualización Exitosa:** Se actualiza la nota. | `204 No Content` | |
| **`PUT /notes/{id}`** | **No Encontrado:** La nota a actualizar no existe. | `404 Not Found` | |
| **`DELETE /notes/{id}`** | **Eliminación Exitosa:** Se elimina la nota. | `204 No Content` | |
| **`DELETE /notes/{id}`** | **No Encontrado:** La nota a eliminar no existe. | `404 Not Found` | |

### II. Tests Unitarios para el Service (`NotesService`)

El objetivo es probar la **lógica de negocio** y la **interacción con la DB** (mockeando el `DbContext`).

| Operación | Escenario (Lógica de Negocio) | Comprobación (Moq) |
| :--- | :--- | :--- |
| **`CreateNoteAsync`** | Se llama al método. | Verificar que se llama a `_dbContext.Notes.Add()` y `_dbContext.SaveChangesAsync()`. |
| **`UpdateNoteAsync`** | La nota existe y se actualiza. | Verificar que se llama a `_dbContext.Notes.Update()` y `_dbContext.SaveChangesAsync()`. |
| **`DeleteNoteAsync`** | La nota existe y se elimina. | Verificar que se llama a `_dbContext.Notes.Remove()` y `_dbContext.SaveChangesAsync()`. |
| **`DeleteNoteAsync`** | La nota no existe. | Verificar que no se llama a `_dbContext.SaveChangesAsync()` y que devuelve `false`. |
