export const COLOR_HEX_BY_NAME: Record<string, string> = {
  lila: '#C8A2C8',
  negro: '#1a1a1a',
  rosado: '#f9a8d4',
  blanco: '#ffffff',
  marron: '#92400e',
  marrón: '#92400e',
  verde: '#16a34a',
  celeste: '#7dd3fc',
  rojo: '#dc2626',
  plomo: '#6b7280',
  camel: '#c19a6b',
  perla: '#f0e6d2',
  'azul marino': '#1e3a5f',
  'azul-marino': '#1e3a5f',
  azul: '#2563eb',
  gris: '#9ca3af',
  beige: '#d6b98c',
  crema: '#f5f5dc',
  nude: '#e5d3b3',
};

export function getColorHex(colorNombre?: string): string {
  if (!colorNombre) {
    return '#9ca3af';
  }

  const clave = colorNombre.trim().toLowerCase();
  return COLOR_HEX_BY_NAME[clave] ?? '#9ca3af';
}

export function getColorBorderClass(colorNombre?: string): string {
  const hex = getColorHex(colorNombre);
  const coloresClaros = ['#ffffff', '#f0e6d2', '#f5f5dc', '#e5d3b3', '#f9a8d4', '#d6b98c'];
  const isClaro = coloresClaros.includes(hex.toLowerCase());

  return isClaro ? 'border-2 border-gray-300' : '';
}
