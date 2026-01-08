import { IOCContainer } from "@/bootstrapper/constants/IOCContainer";
import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { IAppSnackbarService } from "@/spi/AppSnacbarSPI";
import { useEffect, useState } from "react";

export default function useAppSnackbarViewModel() {
    const appSnackbarService = IOCContainer.get<IAppSnackbarService>(ServicesIdentifiers.AppSnackbarService);
    const [visibility, setVisibility] = useState(false);
    const [messageList, setMessageList] = useState<string[]>([]);
    const [message, setMessage] = useState<string>("");

    useEffect(() => {
        appSnackbarService.subscribe((message: string) => {
            setMessageList(oldArray => [...oldArray, message]);
        });
    }, [])

    function onDismiss() {
        setVisibility(false);
        setTimeout(() => {
            setMessageList(prevList => {
                if (prevList.length > 0) {
                    const [nextMessage, ...remainingMessages] = prevList;
                    setMessage(nextMessage);
                    setVisibility(true);
                    return remainingMessages;
                }
                return prevList;
            });
        }, 300);
    }

    useEffect(() => {
        if (visibility || messageList.length === 0) {
            return;
        }
        const [firstMessage, ...remainingMessages] = messageList;
        setMessage(firstMessage);
        setMessageList(remainingMessages);
        setVisibility(true);
    }, [messageList, message, visibility]);

    return { visibility, message, onDismiss };
}