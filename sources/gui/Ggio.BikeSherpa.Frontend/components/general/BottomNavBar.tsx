import { Link, usePathname } from 'expo-router';
import { View, StyleSheet } from 'react-native';
import { Icon, Text, useTheme } from 'react-native-paper';
import {useSafeAreaInsets} from "react-native-safe-area-context";

type NavItem = {
    href: string;
    title: string;
    icon: string;
};

type Props = {
    items: NavItem[];
};

export function BottomNavBar({ items }: Props) {
    const theme = useTheme();
    const areaInsets = useSafeAreaInsets();
    const pathname = usePathname();

    return (
        <View style={[styles.bar, { backgroundColor: theme.colors.surface, marginBottom: areaInsets.bottom }]}>
            {items.map((item) => {
                const isActive = pathname.startsWith(item.href.replace('/(tabs)/', '/'));
                return (
                    <View
                        key={item.href}
                        style={[
                            styles.tab,
                            isActive && { backgroundColor: theme.colors.primary }
                        ]}
                    >
                        <Link href={item.href as any} style={styles.link}>
                            <View style={styles.tabInner}>
                                <Icon
                                    source={item.icon}
                                    size={28}
                                    color={isActive ? theme.colors.background : theme.colors.onSurface}
                                />
                                <Text
                                    style={[
                                        { fontWeight: "bold"},
                                        { color: isActive ? theme.colors.background : theme.colors.onSurface }
                                    ]}
                                >
                                    {item.title}
                                </Text>
                            </View>
                        </Link>
                    </View>
                );
            })}
        </View>
    );
}

const styles = StyleSheet.create({
    bar: {
        flexDirection: 'row',
        height: 60,
        elevation: 8,
        shadowOpacity: 0.1,
        shadowRadius: 4,
        justifyContent: 'center',
        alignItems: 'center'
    },
    tab: {
        flex: 1,
        borderRadius: 0,
    },
    link: {
        display: 'flex',
        flex: 1,
        width: '100%',
        height: '100%',
        textAlign: "center",
    },
    tabInner: {
        flex: 1,
        alignItems: 'center',
        justifyContent: 'center',
        gap: 2,
    },
});