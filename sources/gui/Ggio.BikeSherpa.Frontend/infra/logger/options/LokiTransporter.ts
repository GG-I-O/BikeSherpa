import { transportFunctionType } from "react-native-logs";

// Type for Loki transport options
export type LokiTransportOptions = {
    host: string;
    labels?: Record<string, string | (() => string)>;
    batching?: {
        enabled: boolean;
        interval: number; // milliseconds
        maxBatchSize: number;
    };
};

// Type for log batch entry
type LogBatchEntry = {
    timestamp: string;
    line: string;
    level: string;
};

// Loki custom transport factory
export const createLokiTransport = (
    lokiOptions: LokiTransportOptions
): transportFunctionType<{}> => {
    let logBatch: LogBatchEntry[] = [];
    let batchTimeout: ReturnType<typeof setTimeout> | null = null;

    // Resolve label if it is a function, for dynamic labels (ex: usernames)
    const resolveLabels = (labels?: Record<string, string | (() => string)>): Record<string, string> => {
        const resolved: Record<string, string> = {};
        if (labels) {
            Object.entries(labels).forEach(([key, value]) => {
                if (typeof value === 'function') {
                    resolved[key] = value();
                } else {
                    resolved[key] = value;
                }
            });
        }
        return resolved;
    };

    const sendBatchToLoki = async (batch: LogBatchEntry[]): Promise<void> => {
        if (batch.length === 0)
            return;
        try {
            const resolvedLabels = resolveLabels(lokiOptions.labels);

            // Format logs for Loki push API
            const payload = {
                streams: [
                    {
                        stream: resolvedLabels,
                        values: batch.map((log) => [
                            log.timestamp,
                            JSON.stringify({
                                level: log.level,
                                message: log.line,
                            }),
                        ]),
                    },
                ],
            };

            // Post logs to loki
            const response = await fetch(`${lokiOptions.host}/api/v1/push`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(payload),
            });

            if (!response.ok) {
                console.error(
                    `Failed to send logs to Loki: ${response.status} ${response.statusText}`
                );
                const errorText = await response.text();
                console.error("Loki error response:", errorText);
            }
        } catch (error) {
            console.error("Error sending logs to Loki:", error);
        }
    };

    const flushBatch = (): void => {
        if (logBatch.length > 0) {
            sendBatchToLoki([...logBatch]);
            logBatch = [];
        }
        if (batchTimeout) {
            clearTimeout(batchTimeout);
            batchTimeout = null;
        }
    };

    return (props): void => {
        const { rawMsg, level, extension } = props;

        // Convert timestamp to nanoseconds (Loki format)
        const timestamp = Date.now() * 1000 * 1000;

        // Format the log message
        let logMessage = "";
        if (Array.isArray(rawMsg)) {
            logMessage = rawMsg.map((m) => JSON.stringify(m)).join(" ");
        } else if (typeof rawMsg === "object" && rawMsg !== null) {
            logMessage = JSON.stringify(rawMsg);
        } else {
            logMessage = String(rawMsg);
        }

        // Add extension (namespace) if present
        if (extension) {
            logMessage = `[${extension}] ${logMessage}`;
        }

        // Check if batching is enabled
        if (lokiOptions.batching?.enabled) {
            logBatch.push({
                timestamp: timestamp.toString(),
                line: logMessage,
                level: level.text,
            });

            // Send batch if max size reached
            if (logBatch.length >= (lokiOptions.batching.maxBatchSize || 10)) {
                flushBatch();
            } else {
                // Schedule batch send if not already scheduled
                if (!batchTimeout) {
                    batchTimeout = setTimeout(() => {
                        flushBatch();
                    }, lokiOptions.batching.interval || 5000);
                }
            }
        } else {
            // Send immediately if batching is disabled
            sendBatchToLoki([
                {
                    timestamp: timestamp.toString(),
                    line: logMessage,
                    level: level.text,
                },
            ]);
        }
    };
};