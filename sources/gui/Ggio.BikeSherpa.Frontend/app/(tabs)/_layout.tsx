import { HapticTab } from '@/components/general/HapticTab';
import { Tabs } from 'expo-router';
import { Icon, useTheme } from 'react-native-paper';

export default function TabLayout() {
    const theme = useTheme();

    return (
        <Tabs
            screenOptions={{
                tabBarActiveTintColor: theme.colors.background,
                tabBarActiveBackgroundColor: theme.colors.primary,
                headerShown: false,
                tabBarButton: HapticTab,
                tabBarLabelStyle: { fontWeight: '600' }
            }}>
            <Tabs.Screen
                name="(deliveries)"
                options={{
                    href: "/(tabs)/(deliveries)",
                    title: 'Courses',
                    tabBarIcon: ({ color }) => <Icon source="calendar" size={28} color={color} />

                }}
            />
            <Tabs.Screen
                name="(myDeliveries)"
                options={{
                    href: "/(tabs)/(myDeliveries)",
                    title: 'Mes courses',
                    tabBarIcon: ({ color }) => <Icon source="bicycle-cargo" size={28} color={color} />

                }}
            />
            <Tabs.Screen
                name="(couriers)"
                options={{
                    href: "/(tabs)/(couriers)",
                    title: 'Livreurs',
                    tabBarIcon: ({ color }) => <Icon source="account-box-multiple" size={28} color={color} />

                }}
            />
            <Tabs.Screen
                name="(customers)"
                options={{
                    href: "/(tabs)/(customers)",
                    title: 'Clients',
                    tabBarIcon: ({ color }) => <Icon source="card-account-details" size={28} color={color} />

                }}
            />
            <Tabs.Screen
                name="(reports)"
                options={{
                    href: "/(tabs)/(reports)",
                    title: 'Rapports',
                    tabBarIcon: ({ color }) => <Icon source="file-document" size={28} color={color} />

                }}
            />
        </Tabs>
    );

}