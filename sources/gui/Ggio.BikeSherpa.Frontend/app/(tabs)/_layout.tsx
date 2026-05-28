import {HapticTab} from '@/components/general/HapticTab';
import {Tabs} from 'expo-router';
import {Icon, useTheme} from 'react-native-paper';
import {IAuthService} from "@/spi/AuthSPI";
import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import {useEffect, useState} from "react";
import {useAuth0} from "react-native-auth0";

export default function TabLayout() {
    const theme = useTheme();

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

    return (
        <Tabs
            initialRouteName={userIsDispatcher ? "(deliveries)" : "(myDeliveries)"}
            screenOptions={{
                tabBarActiveTintColor: theme.colors.background,
                tabBarActiveBackgroundColor: theme.colors.primary,
                headerShown: false,
                tabBarButton: HapticTab,
                tabBarLabelStyle: {fontWeight: '600'}
            }}>
            <Tabs.Screen
                name="(myDeliveries)"
                options={{
                    href: userIsDispatcher ? null : "/(tabs)/(myDeliveries)",
                    title: 'Mes courses',
                    tabBarIcon: ({color}) => <Icon source="bicycle-cargo" size={28} color={color}/>
                }}
            />
            <Tabs.Screen
                name="(deliveries)"
                options={{
                    href: userIsDispatcher ? "/(tabs)/(deliveries)" : null,
                    title: 'Courses',
                    tabBarIcon: ({color}) => <Icon source="calendar" size={28} color={color}/>
                }}
            />
            <Tabs.Screen
                name="(couriers)"
                options={{
                    href: userIsDispatcher ? "/(tabs)/(couriers)" : null,
                    title: 'Livreurs',
                    tabBarIcon: ({color}) => <Icon source="account-box-multiple" size={28} color={color}/>
                }}
            />
            <Tabs.Screen
                name="(customers)"
                options={{
                    href: userIsDispatcher ? "/(tabs)/(customers)" : null,
                    title: 'Clients',
                    tabBarIcon: ({color}) => <Icon source="card-account-details" size={28} color={color}/>
                }}
            />
            <Tabs.Screen
                name="(reports)"
                options={{
                    href: userIsDispatcher ? "/(tabs)/(reports)" : null,
                    title: 'Rapports',
                    tabBarIcon: ({color}) => <Icon source="file-document" size={28} color={color}/>
                }}
            />
            <Tabs.Screen
                name="(profile)"
                options={{
                    href: "/(tabs)/(profile)",
                    title: 'Profil',
                    tabBarIcon: ({color}) => <Icon source="account" size={28} color={color}/>
                }}
            />
        </Tabs>
    );
}