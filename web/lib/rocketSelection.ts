export function toggleRocketId(selectedIds: number[], id: number, max = 4) {
  if (selectedIds.includes(id)) return selectedIds.filter((selectedId) => selectedId !== id);
  return selectedIds.length < max ? [...selectedIds, id] : selectedIds;
}
