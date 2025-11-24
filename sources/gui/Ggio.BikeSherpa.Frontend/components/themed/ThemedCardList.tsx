import { Identifiable } from '@/data/Identifiable'
import React, { ReactElement } from 'react'
import { FlatList, ListRenderItem, StyleProp, View, ViewStyle } from 'react-native'
import { useSafeAreaInsets } from 'react-native-safe-area-context'

type Props = {
    data: Array<Identifiable<string | number>>,
    searchBar?: ReactElement,
    card: ListRenderItem<Identifiable<string | number>> | null | undefined,
    style?: StyleProp<ViewStyle>,
    numColumns?: number | undefined
}

export default function ThemedCardList({ data, searchBar, card, style, numColumns }: Props) {
    const safeInset = useSafeAreaInsets();

    return (
        <View style={{ margin: 8, gap: 8, flex: 1 }}>

            {searchBar}

            <FlatList
                ListFooterComponent={
                    <View style={{ height: safeInset.bottom * 2 }} />
                }
                data={data}
                contentContainerStyle={[style, { gap: 8 }]}
                columnWrapperStyle={{ gap: 8 }}
                keyExtractor={item => item.id.toString()}
                renderItem={card}
                numColumns={numColumns}
            />
        </View>
    )
}