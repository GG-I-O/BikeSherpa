import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import Customer from "@/customers/models/Customer";
import { IAppSnackbarService } from "@/spi/AppSnacbarSPI";
import { IStorageContext } from "@/spi/StorageSPI";
import { inject, injectable } from "inversify";
import { EventRegister } from "react-native-event-listeners";

@injectable()
export default class AppSnackbarService implements IAppSnackbarService {
    private readonly eventType = "snackbar";

    public constructor(
        @inject(ServicesIdentifiers.CustomerStorage) customerStorage: IStorageContext<Customer>
    ) {
        customerStorage.subscribeToOnErrorEvent((data) => this.emit(data));
    }

    public subscribe(callback: (message: string) => void): string {
        const eventId = EventRegister.addEventListener(this.eventType, callback);
        if (eventId) {
            return eventId as string;
        }
        return "";
    }

    public unSubscribe(id: string): void {
        EventRegister.removeEventListener(id);
    };

    private emit(data: string) {
        EventRegister.emit(this.eventType, data);
    }
}