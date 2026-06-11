import {useTheme} from 'react-native-paper';
import {IAuthService} from "@/spi/AuthSPI";
import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import {useEffect, useState} from "react";
import {useAuth0} from "react-native-auth0";
import {BottomNavBar} from "@/components/general/BottomNavBar";
import {Stack} from "expo-router";

export default function TabLayout() {
    const [userIsDispatcher, setUserIsDispatcher] = useState<boolean | null>(null);
    const [authError, setAuthError] = useState(false);

    const authService = IOCContainer.get<IAuthService>(ServicesIdentifiers.AuthService);
    const {clearSession} = useAuth0();

    useEffect(() => {
        authService.isDispatcher()
            .then((result) => setUserIsDispatcher(result))
            .catch((error) => {
                console.error("Error verifying roles : ", error);
                setAuthError(true);
            })
    }, [authService, setUserIsDispatcher]);

    useEffect(() => {
        if (!authError) return;
        const logout = async () => {
            try {
                await clearSession();
            } catch (e) {
                console.error("Logout failed:", e);
            }
        };
        logout().then();
    }, [authError, clearSession]);

    if (userIsDispatcher === null)
        return <></>;

    const dispatcherItems = [
        {href: '/(tabs)/deliveries', title: 'Courses', icon: 'calendar'},
        {href: '/(tabs)/couriers', title: 'Livreurs', icon: 'account-box-multiple'},
        {href: '/(tabs)/customers', title: 'Clients', icon: 'card-account-details'},
        {href: '/(tabs)/reports', title: 'Rapports', icon: 'file-document'},
        {href: '/(tabs)/profile', title: 'Profil', icon: 'account'},
    ];

    const courierItems = [
        {href: '/(tabs)/myDeliveries', title: 'Mes courses', icon: 'bicycle-cargo'},
        {href: '/(tabs)/profile', title: 'Profil', icon: 'account'},
    ];

    const items = userIsDispatcher ? dispatcherItems : courierItems;
    const initialRoute = userIsDispatcher ? "deliveries" : "myDeliveries";

    return (
        <>
            <Stack screenOptions={{headerShown: false}} initialRouteName={initialRoute}/>
            <BottomNavBar items={items}/>
        </>
    );
}