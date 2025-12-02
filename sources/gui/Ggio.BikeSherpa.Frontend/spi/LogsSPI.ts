export interface ILogger {
    error: (...args: unknown[]) => void;
    warn: (...args: unknown[]) => void;
    info: (...args: unknown[]) => void;
    debug: (...args: unknown[]) => void;
    extend: (extension: string) => ILogger;
}

export interface ILoggerConfig {
    host: string;
    app: string;
    platform: string;
}

