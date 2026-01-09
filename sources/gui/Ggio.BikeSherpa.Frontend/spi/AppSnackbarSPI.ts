export interface IAppSnackbarService {
    subscribe: (callback: (message: string) => void) => string;
    unSubscribe: (id: string) => void;
}