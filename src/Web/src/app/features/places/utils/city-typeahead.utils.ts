/**
 * Strips a trailing " (country)" label from the city typeahead so filters store the plain city
 * name that matches persisted place data.
 */
export function extractCityNameFromTypeaheadValue(value: string): string {
  const trimmed = value.trim();
  const match = trimmed.match(/^(.+?)\s+\((.+)\)$/);
  if (match) {
    return match[1].trim();
  }
  return trimmed;
}
