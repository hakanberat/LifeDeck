
export async function saveJson(json, fileName) {
    const blob = new Blob([json], {
        type: "application/json;charset=utf-8"
    });

    // Use the native Save As dialog when supported.
    if (typeof window.showSaveFilePicker === "function") {
        try {
            const handle = await window.showSaveFilePicker({
                suggestedName: fileName,
                types: [
                    {
                        description: "JSON backup",
                        accept: {
                            "application/json": [".json"]
                        }
                    }
                ]
            });

            const writable = await handle.createWritable();

            await writable.write(blob);
            await writable.close();

            return "saved";
        }
        catch (error) {
            if (error.name === "AbortError") {
                return "cancelled";
            }

            // Browser security restrictions may prevent
            // opening the native file picker.
            if (
                error.name !== "SecurityError" &&
                error.name !== "NotAllowedError"
            ) {
                throw error;
            }
        }
    }

    // Fallback: download the backup file.
    const url = URL.createObjectURL(blob);

    const link = document.createElement("a");
    link.href = url;
    link.download = fileName;

    document.body.appendChild(link);
    link.click();
    link.remove();

    setTimeout(() => URL.revokeObjectURL(url), 1000);

    return "downloaded";
}

export function restoreData(dataToRestore) {
    const keys = Object.keys(dataToRestore);
    const previousValues = new Map();
    const updatedKeys = [];

    // Preserve original browser storage values in memory.
    for (const key of keys) {
        previousValues.set(
            key,
            localStorage.getItem(key)
        );
    }

    try {
        // Replace records for all six modules.
        for (const key of keys) {
            localStorage.setItem(
                key,
                dataToRestore[key]
            );

            updatedKeys.push(key);
        }

        return "restored";
    }
    catch (error) {
        console.error("Restore failed:", error);

        // Attempt to recover the original records.
        const rollbackFailures = [];

        for (const key of updatedKeys.reverse()) {
            try {
                const original = previousValues.get(key);

                if (original === null) {
                    localStorage.removeItem(key);
                } else {
                    localStorage.setItem(key, original);
                }
            }
            catch (rollbackError) {
                console.error(
                    `Unable to recover ${key}:`,
                    rollbackError
                );

                rollbackFailures.push(key);
            }
        }

        return rollbackFailures.length > 0
            ? "rollback-failed"
            : "failed";
    }
}

export async function saveFile(
    bytes,
    fileName,
    mimeType,
    extension
) {
    const blob = new Blob([bytes], {
        type: mimeType
    });

    if (typeof window.showSaveFilePicker === "function") {
        try {
            const handle = await window.showSaveFilePicker({
                suggestedName: fileName,
                types: [{
                    description: `${extension.toUpperCase()} file`,
                    accept: {
                        [mimeType.split(";")[0]]: [`.${extension}`]
                    }
                }]
            });

            const writable = await handle.createWritable();

            await writable.write(blob);
            await writable.close();

            return "saved";
        }
        catch (error) {
            if (error.name === "AbortError") {
                return "cancelled";
            }

            if (
                error.name !== "SecurityError" &&
                error.name !== "NotAllowedError"
            ) {
                throw error;
            }
        }
    }

    const url = URL.createObjectURL(blob);

    const link = document.createElement("a");
    link.href = url;
    link.download = fileName;

    document.body.appendChild(link);
    link.click();
    link.remove();

    setTimeout(() => URL.revokeObjectURL(url), 1000);

    return "downloaded";
}
