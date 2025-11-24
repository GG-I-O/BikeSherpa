/**
 * Learn more about light and dark modes:
 * https://docs.expo.dev/guides/color-schemes/
 */

import { ColorPalette } from '@/constants/Colors';
import { useThemeColors } from './useThemeColors';

export function useThemeColor(colorName: keyof ColorPalette) {
  const colors = useThemeColors();
  return colors[colorName] ?? colors.tint;
}
